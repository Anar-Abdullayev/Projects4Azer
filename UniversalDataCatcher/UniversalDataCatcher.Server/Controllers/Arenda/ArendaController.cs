using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Controllers.Arenda
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArendaController : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult StartService([FromBody] StartBotRequestDto requestDto)
        {

            if (ArendaService.IsRunning)
                return BadRequest("Service is already running.");
            if (requestDto.RepeatEveryMinutes is null)
                requestDto.RepeatEveryMinutes = 30;
            ArendaService.Start(requestDto.DayDifference, (int) requestDto.RepeatEveryMinutes);
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
