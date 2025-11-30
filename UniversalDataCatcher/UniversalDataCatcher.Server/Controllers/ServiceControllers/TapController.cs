using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Bots.Tap.Services;

namespace UniversalDataCatcher.Server.Controllers.ServiceControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TapController(TapAzService service) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (service.IsRunning)
                return BadRequest("Service is already running.");

            service.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!service.IsRunning)
                return BadRequest("Service is not running.");
            service.Stop();
            return Ok("Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                service.Progress,
                service.IsRunning,
                service.SleepTime,
                service.StartTime
            };
            return Ok(response);
        }
    }
}
