using Microsoft.AspNetCore.Mvc;
using StackOverflowLoaderAPI.Data;
using StackOverflowLoaderAPI.Repositories;

namespace StackOverflowLoaderAPI.Controllers
{

    // base address: api/Administrative
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrativeController : ControllerBase
    {
        private readonly ILogger<AdministrativeController> _logger;
        private readonly IItemRepository repo;

        public AdministrativeController(ILogger<AdministrativeController> logger, IItemRepository repo)
        {
            _logger = logger;
            this.repo = repo;

        }

        // PUT: api/Administrative/forceupdate
        [HttpPut]
        public async Task<IActionResult> forceUpdate()
        {
            _logger.LogInformation("Forcing Tags update");            
            await repo.forceUpdate(_logger);            
            

            return Ok();
        }
    }
}
