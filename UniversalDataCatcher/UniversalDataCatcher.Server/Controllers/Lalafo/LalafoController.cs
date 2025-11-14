using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;

namespace UniversalDataCatcher.Server.Controllers.Lalafo
{
    [Route("api/[controller]")]
    [ApiController]
    public class LalafoController : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start()
        {
            if (LalafoService.IsRunning)
                return BadRequest("Service is already running.");

            LalafoService.Start(1, 30);

            return Ok("Lalafo Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!LalafoService.IsRunning)
                return BadRequest("Service is not running.");
            LalafoService.Stop();
            return Ok("Lalafo Service Stopped");
        }
    }
}
