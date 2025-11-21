using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Arenda.DTOs;
using UniversalDataCatcher.Server.Bots.EvTen.Services;

namespace UniversalDataCatcher.Server.Controllers.EvTen
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvTenController(EvTenService evTenService) : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult Start([FromBody] StartBotRequestDto request)
        {
            if (evTenService.IsRunning)
                return BadRequest("Service is already running.");

            evTenService.Start(request.DayDifference, (int)request.RepeatEveryMinutes!);

            return Ok("Ev10 Service Started");
        }

        [HttpPost("stop")]
        public ActionResult Stop()
        {
            if (!evTenService.IsRunning)
                return BadRequest("Service is not running.");
            evTenService.Stop();
            return Ok("Lalafo Service Stopped");
        }


        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            var response = new
            {
                evTenService.Progress,
                evTenService.IsRunning
            };
            return Ok(response);
        }
    }
}
