using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Dtos;
using UniversalDataCatcher.Server.Entities;
using UniversalDataCatcher.Server.Interfaces;

namespace UniversalDataCatcher.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private IAdvertisementService _advertisementService;
        public PostsController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
            _advertisementService.SetTable("post");
        }
        [HttpPost]
        public async Task<ActionResult> GetPosts([FromBody] AdvertisementFilter? filter)
        {
            var result = await _advertisementService.GetAllAsync(filter);
            return Ok(result);
        }
    }
}
