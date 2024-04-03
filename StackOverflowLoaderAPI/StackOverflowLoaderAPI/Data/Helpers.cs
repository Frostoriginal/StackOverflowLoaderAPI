using Microsoft.EntityFrameworkCore;
using StackOverflowLoaderAPI.Models.ApiTags;
using StackOverflowLoaderAPI.Models.StackOverflowTags;
using static System.Net.Mime.MediaTypeNames;
using System.Configuration;

namespace StackOverflowLoaderAPI.Data
{
    public class Helpers
    {        

        /// <summary>
        /// This method will try to connect to mssql server and create db with code first approach. 
        /// Due to docker container spin up time the method will try to connect in regular intervals until connected.
        /// Afterwards method will create the database and test write and delete operations.
        /// </summary>
        /// <returns></returns>
        public static Task connectAndCreateDB(string connString, bool resetDBOnStartup)
        {  
            using (var ctx = new TagDataDataContext(connString))
            {
                //try until database is online and connection is established 
                bool canConnect = false;
                while (canConnect == false)
                {
                    try
                    {
                        Console.WriteLine("Connecting to db");
                        ctx.Database.CanConnect();
                        ctx.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"Exception - cannot connect to DB, message: {ex}");
                        Console.WriteLine($"Cannot connect to DB, trying again in 10s");
                        Console.WriteLine($"Exception message: {ex.Message}");
                        System.Threading.Thread.Sleep(10000);
                        // throw;
                    }
                    System.Threading.Thread.Sleep(10000);
                    if (ctx.Database.CanConnect()) canConnect = true;

                }
                Console.WriteLine("Connection established");
                if (resetDBOnStartup) { 
                    try
                    {
                        Console.WriteLine("Reseting database.");
                        ctx.Database.EnsureDeleted();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not reset database. Exception message: {ex.Message}");                        
                    }
                }

                try
                {
                    Console.WriteLine("Creating database.");
                    ctx.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not create database. Exception message: {ex.Message}");
                    throw;
                }

                Console.WriteLine($"Database created succesfully.");
                bool testItemFound = false;
                Item testItem = new Item() { count = 1, has_synonyms = true, is_moderator_only = true, is_required = true, name = "xxx" };

                //check 
                try
                {
                    ctx.Items.Add(testItem);                    
                    ctx.SaveChanges();

                    //check for test item
                    testItemFound = ctx.Items.ContainsAsync(testItem).Result;
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not add item to database. Exception message:{ex.Message}");
                    
                }
               
                if (testItemFound)
                {
                    Console.WriteLine("Test item properly added to database.");
                    ctx.Items.Remove(testItem);
                    ctx.SaveChanges();
                    //log testItem removal
                    if(ctx.Items.ContainsAsync(testItem).Result == false) Console.WriteLine("Test item properly deleted from database.");
                }

                Console.WriteLine("Database ready.");
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// This method will load selected amount of tags from StackOverflow Api
        /// </summary>
        /// <param name="connString">Database connection string, method runs before registering DBContext</param>
        /// <param name="tagToLoadMinAmount">Minimum amount of tags to load, default is 1000 </param>
        /// <param name="pageSize">Api call page size, default is max accepted - 100</param>
        /// <param name="waitTime">Wait time before api calls to avoid throttle violation</param>
        /// <returns></returns>
        public static async Task loadSomeTags(string connString, int tagToLoadMinAmount = 1000,  int pageSize = 100, int waitTime = 5000)
        {
            List<Item> loadedItems = new();
            int max = 2528821;
            string urlMax = $"https://api.stackexchange.com/2.3/tags?pagesize={pageSize}&order=desc&max={max}&sort=popular&site=stackoverflow";

            while (loadedItems.Count < tagToLoadMinAmount)
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate }))
                {
                    ApiResponse newApiResponse = new ApiResponse();
                    try
                    {                        
                        await Console.Out.WriteLineAsync($"Loading tag data. Currently loaded: {loadedItems.Count}. Load page size: {pageSize}. Target amount: {tagToLoadMinAmount} tags. Next batch in: {waitTime/1000} seconds");
                        newApiResponse = await client.GetFromJsonAsync<ApiResponse>(urlMax);
                        max = newApiResponse.items.Last().count - 1;
                        urlMax = $"https://api.stackexchange.com/2.3/tags?pagesize={pageSize}&order=desc&max={max}&sort=popular&site=stackoverflow";
                        

                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"{ex.Message}");
                    }
                    if (newApiResponse.items.Count > 0) loadedItems.AddRange(newApiResponse.items);
                    
                }
                //wait to not trigger throttle violation

                System.Threading.Thread.Sleep(waitTime);

            }

            using (var ctx = new TagDataDataContext(connString))
            {
                ctx.AddRange(loadedItems);
                ctx.SaveChanges();
                Console.WriteLine($"Succesfully loaded {loadedItems.Count} tags. Database has: {ctx.Items.ToList().Count} tags.");
            }

            calculateThePercentage();

            return;
        }
               
                

        /// <summary>
        /// Method calculates percentage of each tag count among total tag count number.
        /// </summary>
        public static void calculateThePercentage()
        {
            string connString = Environment.GetEnvironmentVariable("connstring");
            using (var ctx = new TagDataDataContext(connString))
            {
                int total = ctx.Items.Sum(item => item.count);

                List<Item> items = ctx.Items.ToList();

                foreach (var item in items)
                {
                item.share = (double)item.count / (double)total * (double)100;
                ctx.Update(item);                   

                }
                ctx.SaveChanges();
            }
        }       



    }




}
