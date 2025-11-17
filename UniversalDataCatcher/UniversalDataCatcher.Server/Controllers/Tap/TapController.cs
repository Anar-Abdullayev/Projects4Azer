using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Bots.Tap.Services;

namespace UniversalDataCatcher.Server.Controllers.Tap
{
    [Route("api/[controller]")]
    [ApiController]
    public class TapController : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (TapAzService.IsRunning)
                return BadRequest("Service is already running.");

            TapAzService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Tapaz Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!TapAzService.IsRunning)
                return BadRequest("Service is not running.");
            TapAzService.Stop();
            return Ok("Tapaz Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                TapAzService.Progress,
                TapAzService.IsRunning
            };
            return Ok(response);
        }
    }
}
