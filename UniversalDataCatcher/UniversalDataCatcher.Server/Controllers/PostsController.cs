using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UniversalDataCatcher.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetPosts()
        {

            return Ok("PostsController is working!");
        }
    }
}
