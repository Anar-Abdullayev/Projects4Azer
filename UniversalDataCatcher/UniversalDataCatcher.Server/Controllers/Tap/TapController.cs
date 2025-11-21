using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Bots.Tap.Services;

namespace UniversalDataCatcher.Server.Controllers.Tap
{
    [Route("api/[controller]")]
    [ApiController]
    public class TapController(TapAzService tapAzService) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (tapAzService.IsRunning)
                return BadRequest("Service is already running.");

            tapAzService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Tapaz Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!tapAzService.IsRunning)
                return BadRequest("Service is not running.");
            tapAzService.Stop();
            return Ok("Tapaz Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                tapAzService.Progress,
                tapAzService.IsRunning
            };
            return Ok(response);
        }
    }
}
