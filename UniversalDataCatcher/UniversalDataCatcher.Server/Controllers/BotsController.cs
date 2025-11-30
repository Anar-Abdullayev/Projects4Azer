using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversalDataCatcher.Server.Bots.Bina.Services;
using UniversalDataCatcher.Server.Bots.Emlak.Services;
using UniversalDataCatcher.Server.Bots.EvTen.Services;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Bots.Tap.Services;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Services;
using UniversalDataCatcher.Server.Controllers.Arenda;
using UniversalDataCatcher.Server.Controllers.Bina;
using UniversalDataCatcher.Server.Controllers.Emlak;
using UniversalDataCatcher.Server.Controllers.EvTen;
using UniversalDataCatcher.Server.Controllers.Lalafo;
using UniversalDataCatcher.Server.Controllers.Tap;
using UniversalDataCatcher.Server.Controllers.YeniEmlak;
using UniversalDataCatcher.Server.Dtos;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotsController(
        ArendaService arendaService,
        TapAzService tapAzService,
        BinaService binaService,
        EmlakService emlakService,
        EvTenService evTenService,
        LalafoService lalafoService,
        YeniEmlakService yeniEmlakService
        ) : ControllerBase
    {
        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            List<StatusResponse> statusResponses = new();

            statusResponses.Add(new StatusResponse { IsRunning = arendaService.IsRunning, Progress = arendaService.Progress, RepeatEvery = arendaService.RepeatEvery, SleepTime = arendaService.SleepTime, ServiceName = nameof(ArendaController).Replace("Controller", "").ToLower(), ServiceLabelName = "Arenda.az" });

            statusResponses.Add(new StatusResponse { IsRunning = tapAzService.IsRunning, Progress = tapAzService.Progress, RepeatEvery = tapAzService.RepeatEvery, SleepTime = tapAzService.SleepTime, ServiceName = nameof(TapController).Replace("Controller", "").ToLower(), ServiceLabelName = "Tap.az" });

            statusResponses.Add(new StatusResponse { IsRunning = binaService.IsRunning, Progress = binaService.Progress, RepeatEvery = binaService.RepeatEvery, SleepTime = binaService.SleepTime, ServiceName = nameof(BinaController).Replace("Controller", "").ToLower(), ServiceLabelName = "Bina.az" });

            statusResponses.Add(new StatusResponse { IsRunning = emlakService.IsRunning, Progress = emlakService.Progress, RepeatEvery = emlakService.RepeatEvery, SleepTime = emlakService.SleepTime, ServiceName = nameof(EmlakController).Replace("Controller", "").ToLower(), ServiceLabelName = "Emlak.az" });

            statusResponses.Add(new StatusResponse { IsRunning = evTenService.IsRunning, Progress = evTenService.Progress, RepeatEvery = evTenService.RepeatEvery, SleepTime = evTenService.SleepTime, ServiceName = nameof(EvTenController).Replace("Controller", "").ToLower(), ServiceLabelName = "Ev10.az" });

            statusResponses.Add(new StatusResponse { IsRunning = lalafoService.IsRunning, Progress = lalafoService.Progress, RepeatEvery = lalafoService.RepeatEvery, SleepTime = lalafoService.SleepTime, ServiceName = nameof(LalafoController).Replace("Controller", "").ToLower(), ServiceLabelName = "Lalafo.az" });

            statusResponses.Add(new StatusResponse { IsRunning = yeniEmlakService.IsRunning, Progress = yeniEmlakService.Progress, RepeatEvery = yeniEmlakService.RepeatEvery, SleepTime = yeniEmlakService.SleepTime, ServiceName = nameof(YeniemlakController).Replace("Controller", "").ToLower(), ServiceLabelName = "YeniEmlak.az" });

            return Ok(statusResponses);
        }
    }
}
