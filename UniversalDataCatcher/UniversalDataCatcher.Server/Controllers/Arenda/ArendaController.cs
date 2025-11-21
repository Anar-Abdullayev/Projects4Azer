using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Controllers.Arenda
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArendaController(ArendaService arendaService) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult StartService([FromBody] StartBotRequestDto requestDto)
        {
            if (arendaService.IsRunning)
                return BadRequest("Service is already running.");
            if (requestDto.RepeatEveryMinutes is null)
                requestDto.RepeatEveryMinutes = 30;
            arendaService.Start(requestDto.DayDifference, (int) requestDto.RepeatEveryMinutes);
            return Ok("Arenda Service Started");
        }

        [HttpPost("stop")]
        public ActionResult StopService()
        {
            if (!arendaService.IsRunning)
                return BadRequest("Service is not running.");
            arendaService.Stop();
            return Ok("Arenda Service Stopped");
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new {
                arendaService.Progress,
                arendaService.IsRunning
            };
            return Ok(response);
        }
    }
}
