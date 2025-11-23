using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Controllers.Arenda
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArendaController(ArendaService service) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult StartService([FromBody] StartBotRequestDto requestDto)
        {
            if (service.IsRunning)
                return BadRequest("Service is already running.");
            if (requestDto.RepeatEveryMinutes is null)
                requestDto.RepeatEveryMinutes = 30;
            service.Start(requestDto.DayDifference, (int) requestDto.RepeatEveryMinutes);
            return Ok("Service Started");
        }

        [HttpPost("stop")]
        public ActionResult StopService()
        {
            if (!service.IsRunning)
                return BadRequest("Service is not running.");
            service.Stop();
            return Ok("Service Stopped");
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new {
                service.Progress,
                service.IsRunning,
                service.SleepTime,
                service.StartTime
            };
            return Ok(response);
        }
    }
}
