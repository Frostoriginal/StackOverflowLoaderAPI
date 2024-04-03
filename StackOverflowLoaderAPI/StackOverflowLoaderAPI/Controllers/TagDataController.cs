using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackOverflowLoaderAPI.Data;
using StackOverflowLoaderAPI.Models.ApiTags;
using StackOverflowLoaderAPI.Models.StackOverflowTags;
using StackOverflowLoaderAPI.Repositories;

namespace StackOverflowLoaderAPI.Controllers;

// base address: api/TagData
[Route("api/[controller]")]
[ApiController]
public class TagDataController:ControllerBase
{
    private readonly ILogger<TagDataController> _logger;
    private readonly IItemRepository repo;
        
    public TagDataController(ILogger<TagDataController> logger, IItemRepository repo)
    {
        _logger = logger;   
        this.repo = repo;
    }
    
    // GET: api/Tagdata
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Item>))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetTags([FromQuery] ItemParameters itemParameters)
    {        
        try
        {            
            var tags = repo.GetAllTagsOrderedAndPaged(itemParameters);

            var metadata = new
            {
                tags.TotalCount,
                tags.PageSize,
                tags.CurrentPage,
                tags.TotalPages,
                tags.HasNext,
                tags.HasPrevious

            };
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            if (tags.CurrentPage > tags.TotalPages) return BadRequest();
            if (tags.Count == 0) return NotFound();

            
            _logger.LogInformation($"Returned {tags.Count()} tags from database.");
            return Ok(tags); // 200 OK with list in body
        }
        catch (Exception ex)
        {
            _logger.LogError($"Bad request. Exception message: {ex.Message}");
            return BadRequest();
        }
        
    }




}
