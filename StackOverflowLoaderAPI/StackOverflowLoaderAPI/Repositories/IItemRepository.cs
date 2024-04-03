using StackOverflowLoaderAPI.Models;
using StackOverflowLoaderAPI.Models.ApiTags;
using StackOverflowLoaderAPI.Models.StackOverflowTags;

namespace StackOverflowLoaderAPI.Repositories;

public interface IItemRepository
{
    Task<Item?> CreateAsync(Item i);
    Task<IEnumerable<Item>> RetrieveAllAsync();
    Task<Item?> RetrieveAsync(int id);
    Task<Item?> UpdateAsync(int id, Item i);
    Task<bool?> DeleteAsync(int id);
    PagedList<Item> GetTags(ItemParameters itemParameters);

    PagedList<Item> GetAllTagsOrderedAndPaged(ItemParameters itemParameters);

    Task<bool> forceUpdate(ILogger logger, int pageSize = 100, int waitTime = 5000);
}
