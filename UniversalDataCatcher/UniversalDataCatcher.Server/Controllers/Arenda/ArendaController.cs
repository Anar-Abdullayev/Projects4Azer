using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Controllers.Arenda
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArendaController : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult StartService()
        {
            if (ArendaService.IsRunning)
                return BadRequest("Service is already running.");
            ArendaService.Start(1);
            return Ok("Arenda Service Started");
        }

        [HttpPost("stop")]
        public ActionResult StopService()
        {
            if (!ArendaService.IsRunning)
                return BadRequest("Service is not running.");
            ArendaService.Stop();
            return Ok("Arenda Service Stopped");
        }
    }
}
