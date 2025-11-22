using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Services;

namespace UniversalDataCatcher.Server.Controllers.YeniEmlak
{
    [Route("api/[controller]")]
    [ApiController]
    public class YeniemlakController(YeniEmlakService yeniEmlakService) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (yeniEmlakService.IsRunning)
                return BadRequest("Service is already running.");

            yeniEmlakService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!yeniEmlakService.IsRunning)
                return BadRequest("Service is not running.");
            yeniEmlakService.Stop();
            return Ok("Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                yeniEmlakService.Progress,
                yeniEmlakService.IsRunning
            };
            return Ok(response);
        }
    }
}
