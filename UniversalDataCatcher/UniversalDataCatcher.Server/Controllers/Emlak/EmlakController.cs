using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.Emlak.Services;

namespace UniversalDataCatcher.Server.Controllers.Emlak
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmlakController(EmlakService emlakService) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (emlakService.IsRunning)
                return BadRequest("Service is already running.");

            emlakService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!emlakService.IsRunning)
                return BadRequest("Service is not running.");
            emlakService.Stop();
            return Ok("Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                emlakService.Progress,
                emlakService.IsRunning
            };
            return Ok(response);
        }
    }
}
