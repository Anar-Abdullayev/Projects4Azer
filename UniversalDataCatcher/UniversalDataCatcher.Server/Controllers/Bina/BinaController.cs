using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.Bina.Services;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;

namespace UniversalDataCatcher.Server.Controllers.Bina
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinaController(BinaService binaService) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (binaService.IsRunning)
                return BadRequest("Service is already running.");

            binaService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Bina Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!binaService.IsRunning)
                return BadRequest("Service is not running.");
            binaService.Stop();
            return Ok("Bina Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                binaService.Progress,
                binaService.IsRunning
            };
            return Ok(response);
        }
    }
}
