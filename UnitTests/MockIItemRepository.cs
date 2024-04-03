using Moq;
using StackOverflowLoaderAPI.Data;
using StackOverflowLoaderAPI.Models.ApiTags;
using StackOverflowLoaderAPI.Models.StackOverflowTags;
using StackOverflowLoaderAPI.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Mocks;

public class MockIItemRepository
{
    private static ConcurrentDictionary<int, Item>? itemCache;

    public static Mock<IItemRepository> GetMock(ItemParameters parameters)
    {
        var mock = new Mock<IItemRepository>();
        List<Item> testItems = GetTestTags();       

        itemCache = new ConcurrentDictionary<int, Item>(testItems.ToDictionary(c => c.Id));


        var tags = GetAllTagsOrderedAndPaged(parameters);        
        mock.Setup(u => u.GetAllTagsOrderedAndPaged(parameters)).Returns(() => tags);

        
        return mock;
    }

   public static PagedList<Item> GetAllTagsOrderedAndPaged(ItemParameters itemParameters) =>
        
        (itemParameters.OrderBy, itemParameters.OrderDirection) switch
        {
            ("name", "ascending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderBy(on => on.name).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),
            ("name", "descending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderByDescending(on => on.name).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),
            ("count", "ascending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderBy(on => on.count).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),
            ("count", "descending") => PagedList<Item>.ToPagedList((itemCache is null ? Enumerable.Empty<Item>() : itemCache.Values.OrderByDescending(on => on.count).ToList()).AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),
            (_, _) => PagedList<Item>.ToPagedList(Enumerable.Empty<Item>().AsQueryable(), itemParameters.PageNumber, itemParameters.PageSize),
        };


   

    public static List<Item> GetTestTags()
    {
        var testTags = new List<Item>();
        testTags.Add(new Item { Id = 1, name = "Demo1", count = 1 });
        testTags.Add(new Item { Id = 2, name = "Demo2", count = 2 });
        testTags.Add(new Item { Id = 3, name = "Demo3", count = 3 });
        testTags.Add(new Item { Id = 4, name = "Demo4", count = 4 });

        return testTags;
    }


}
