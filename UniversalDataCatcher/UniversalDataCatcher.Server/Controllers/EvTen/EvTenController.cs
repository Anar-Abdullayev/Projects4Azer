using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.EvTen.Services;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;

namespace UniversalDataCatcher.Server.Controllers.EvTen
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvTenController : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (EvTenService.IsRunning)
                return BadRequest("Service is already running.");

            EvTenService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Ev10 Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!EvTenService.IsRunning)
                return BadRequest("Service is not running.");
            EvTenService.Stop();
            return Ok("Lalafo Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                EvTenService.Progress,
                EvTenService.IsRunning
            };
            return Ok(response);
        }
    }
}
