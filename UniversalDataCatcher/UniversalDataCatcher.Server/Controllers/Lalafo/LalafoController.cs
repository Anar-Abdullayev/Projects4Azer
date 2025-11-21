using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Controllers.Lalafo
{
    [Route("api/[controller]")]
    [ApiController]
    public class LalafoController(LalafoService lalafoService) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (lalafoService.IsRunning)
                return BadRequest("Service is already running.");

            lalafoService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Lalafo Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!lalafoService.IsRunning)
                return BadRequest("Service is not running.");
            lalafoService.Stop();
            return Ok("Lalafo Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                lalafoService.Progress,
                lalafoService.IsRunning
            };
            return Ok(response);
        }
    }
}
