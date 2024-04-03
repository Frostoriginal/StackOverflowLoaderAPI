namespace UnitTests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StackOverflowLoaderAPI.Controllers;
using StackOverflowLoaderAPI.Models;
using StackOverflowLoaderAPI.Models.ApiTags;
using StackOverflowLoaderAPI.Models.StackOverflowTags;
using StackOverflowLoaderAPI.Repositories;
using System.Net;
using System.Web.Http;
using UnitTests.Mocks;

//ToDo https://learn.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-with-aspnet-web-api
//ToDo https://learn.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/mocking-entity-framework-when-unit-testing-aspnet-web-api-2
//ToDo https://stackoverflow.com/questions/43424095/how-to-unit-test-with-ilogger-in-asp-net-core
public class UnitTest1
{   

    [Fact(DisplayName = "Get call with correct parameters - page size and page number")]
    public void GetAllTagsOrderedAndPaged_ShouldReturnAllItems()
    {
        //Arrange
        ItemParameters parameters = new ItemParameters() { PageNumber = 1, PageSize = 10, OrderBy = "name", OrderDirection = "ascending" };
        var mockItemRepository = MockIItemRepository.GetMock(parameters);
        var logger = Mock.Of<ILogger<TagDataController>>();               
        var controller = new TagDataController(logger, mockItemRepository.Object);
        
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext(),
        };

        // Act
         var result = controller.GetTags(parameters).Result;        
        OkObjectResult response = new(result);
        response = result as OkObjectResult;
        List<Item> responseItems = response.Value as List<Item>;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(MockIItemRepository.GetTestTags().Count,responseItems.Count);

    }

    [Fact(DisplayName = "Get call with correct parameters - order by name, descending")]
    public void GetAllTagsOrderedAndPaged_ShouldReturnNameDescending()
    {
        //Arrange
        ItemParameters parameters = new ItemParameters() { PageNumber = 1, PageSize = 10, OrderBy = "name", OrderDirection = "descending" };
        var mockItemRepository = MockIItemRepository.GetMock(parameters);
        var logger = Mock.Of<ILogger<TagDataController>>();
        var controller = new TagDataController(logger, mockItemRepository.Object);
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext(),
        };

        // Act
        var result = controller.GetTags(parameters).Result;        
        OkObjectResult response = new(result);
        response = result as OkObjectResult;
        List<Item> responseItems = response.Value as List<Item>;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(MockIItemRepository.GetTestTags().OrderByDescending(a=>a.name).First().name, responseItems.First().name);

    }

    [Fact(DisplayName = "Get call with correct parameters - order by count, descending")]
    public void GetAllTagsOrderedAndPaged_ShouldReturnCountDescending()
    {
        //Arrange
        ItemParameters parameters = new ItemParameters() { PageNumber = 1, PageSize = 10, OrderBy = "count", OrderDirection = "descending" };
        var mockItemRepository = MockIItemRepository.GetMock(parameters);
        var logger = Mock.Of<ILogger<TagDataController>>();
        var controller = new TagDataController(logger, mockItemRepository.Object);
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext(),
        };

        // Act
        var result = controller.GetTags(parameters).Result;
        OkObjectResult response = new(result);
        response = result as OkObjectResult;
        List<Item> responseItems = response.Value as List<Item>;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(MockIItemRepository.GetTestTags().OrderByDescending(a => a.count).First().name, responseItems.First().name);

    }



}