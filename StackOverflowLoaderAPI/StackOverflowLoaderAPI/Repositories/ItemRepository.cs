using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using StackOverflowLoaderAPI.Models.StackOverflowTags; //Item
using StackOverflowLoaderAPI.Models.ApiTags;
using StackOverflowLoaderAPI.Data;
using System.Collections.Concurrent; // ConcurrentDictionary


namespace StackOverflowLoaderAPI.Repositories;

public class ItemRepository :IItemRepository 
{
    // Use a static thread-safe dictionary field to cache the customers.
    private static ConcurrentDictionary<int, Item>? itemCache;

    // Use an instance data context field because it should not be
    // cached due to the data context having internal caching.
    private TagDataDataContext db;
    public ItemRepository(TagDataDataContext injectedContext)
    {
        db = injectedContext;
        // Pre-load customers from database as a normal
        // Dictionary with CustomerId as the key,
        // then convert to a thread-safe ConcurrentDictionary.
        if (itemCache is null)
        {
            itemCache = new ConcurrentDictionary<int, Item>(
            db.Items.ToDictionary(c => c.Id));
        }
    }

    public async Task<Item?> CreateAsync(Item i)
    {
        // Normalize CustomerId into uppercase.
        //c.CustomerId = c.CustomerId.ToUpper();
        // Add to database using EF Core.
        EntityEntry<Item> added = await db.Items.AddAsync(i);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            if (itemCache is null) return i;
            // If the customer is new, add it to cache, else
            // call UpdateCache method.
            return itemCache.AddOrUpdate(i.Id, i, UpdateCache);
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Item>> RetrieveAllAsync()
    {
        // For performance, get from cache.
        return Task.FromResult(itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values);
    }
    public Task<Item?> RetrieveAsync(int id)
    {
        // For performance, get from cache.
        //id = id.ToUpper();
        if (itemCache is null) return null!;
        itemCache.TryGetValue(id, out Item? c);
        return Task.FromResult(c);
    }


    private Item UpdateCache(int id, Item i)
    {
        Item? old;
        if (itemCache is not null)
        {
            if (itemCache.TryGetValue(id, out old))
            {
                if (itemCache.TryUpdate(id, i, old))
                {
                    return i;
                }
            }
        }
        return null!;
    }



    public async Task<Item?> UpdateAsync(int id, Item i)
    {
        // Normalize customer Id.
        //id = id.ToUpper();
        //i.CustomerId = c.CustomerId.ToUpper();
        // Update in database.
        db.Items.Update(i);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            // update in cache
            return UpdateCache(id, i);
        }
        return null;
    }
    public async Task<bool?> DeleteAsync(int id)
    {
        //id = id.ToUpper();
        // Remove from database.
        Item? i = db.Items.Find(id);
        if (i is null) return null;
        db.Items.Remove(i);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            if (itemCache is null) return null;
            // Remove from cache.
            return itemCache.TryRemove(id, out i);
        }
        else
        {
            return null;
        }
    }

    public PagedList<Item> GetTags(ItemParameters itemParameters)
    {
        List<Item> items = new List<Item>();

        //string connString = Environment.GetEnvironmentVariable("connstring");
        //using (var ctx = new TagDataDataContext(connString))
        //{
            if (itemParameters.OrderBy.ToLower() == "name")
            {
                //order by name
                if (itemParameters.OrderDirection.ToLower() == "ascending")
                {
                    items = db.Items.OrderBy(on => on.name).ToList();
                }
                else
                {
                    items = db.Items.OrderByDescending(on => on.name).ToList();
                }
            }
            else
            {
                //order by count
                if (itemParameters.OrderDirection.ToLower() == "ascending")
                {
                    items = db.Items.OrderBy(on => on.count).ToList();
                }
                else
                {
                    items = db.Items.OrderByDescending(on => on.count).ToList();
                }
            }
        //}
        return PagedList<Item>.ToPagedList(items.AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize);

    }

    public PagedList<Item> GetAllTagsOrderedAndPaged(ItemParameters itemParameters) =>
    (itemParameters.OrderBy, itemParameters.OrderDirection) switch
        {
            ("name", "ascending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderBy(on => on.name).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize) ,
            ("name", "descending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderByDescending(on => on.name).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize) ,
            ("count", "ascending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderBy(on => on.count).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),
            ("count", "descending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderByDescending(on => on.count).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),
            (_, _) => PagedList<Item>.ToPagedList(Enumerable.Empty<Item>().AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),           
        };

    public async Task<bool> forceUpdate(ILogger logger, int pageSize = 100, int waitTime = 5000)
    {
        bool timemsBeforeNextCall = int.TryParse(Environment.GetEnvironmentVariable("timemsbeforenextcall"), out int msBeforeNextCall);
        if (!timemsBeforeNextCall) msBeforeNextCall = 5000; //in case TryParse fails.
        string connString = Environment.GetEnvironmentVariable("connstring");
        waitTime = msBeforeNextCall;

        int tagsToLoad = 0;
        tagsToLoad = db.Items.Count();
              

        logger.LogInformation("Clearing DB");
        await db.Database.EnsureDeletedAsync();            

        logger.LogInformation("Creating clean DB");
        await db.Database.EnsureCreatedAsync();

        if (db.Items.Count() == 0) logger.LogInformation($"Database cleared");
        

        List<Item> loadedItems = new();
        int max = 2528821;
        string urlMax = $"https://api.stackexchange.com/2.3/tags?pagesize=100&order=desc&max={max}&sort=popular&site=stackoverflow";

        while (loadedItems.Count < tagsToLoad)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate }))
            {
                ApiResponse newApiResponse = new ApiResponse();
                try
                {                    
                    logger.LogInformation($"Loading tag data. Currently loaded: {loadedItems.Count}. Load page size: {pageSize}. Target amount: {tagsToLoad} tags. Next batch in: {waitTime / 1000} seconds");
                    newApiResponse = await client.GetFromJsonAsync<ApiResponse>(urlMax);
                    max = newApiResponse.items.Last().count - 1;
                    urlMax = $"https://api.stackexchange.com/2.3/tags?pagesize={pageSize}&order=desc&max={max}&sort=popular&site=stackoverflow";
                }
                catch (Exception)
                {
                    throw;
                }
                if (newApiResponse.items.Count > 0) loadedItems.AddRange(newApiResponse.items);
                                
            }
            //wait to not trigger throttle violation

            System.Threading.Thread.Sleep(waitTime);

        }
        

        db.AddRange(loadedItems);
        db.SaveChanges();
        logger.LogInformation($"Succesfully loaded {loadedItems.Count} tags. Database has: {db.Items.ToList().Count} tags.");
        itemCache.Clear();        
        itemCache = new ConcurrentDictionary<int, Item>(db.Items.ToDictionary(c => c.Id));        
        Helpers.calculateThePercentage();        

        return true;

    }

}
