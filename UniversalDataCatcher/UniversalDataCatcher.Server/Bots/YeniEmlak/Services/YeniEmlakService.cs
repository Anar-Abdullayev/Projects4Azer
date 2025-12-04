using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Playwright;
using Serilog;
using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Helpers;
using UniversalDataCatcher.Server.Bots.YeniEmlak.StaticConstants;
using UniversalDataCatcher.Server.Helpers;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Bots.YeniEmlak.Services
{
    public class YeniEmlakService : BotService
    {
        private YeniemlakMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;
        public YeniEmlakService(YeniemlakMSSqlDatabaseService _databaseService)
        {
            databaseService = _databaseService;
            logger = LoggerHelper.GetLoggerConfiguration(nameof(YeniEmlakService));
        }

        public void Start(int dayDifference, int repeatEvery)
        {
            if (IsRunning)
                return;
            RepeatEvery = repeatEvery;
            CancellationTokenSource = new CancellationTokenSource();
            IsRunning = true;

            Task.Run(async () =>
            {
                try
                {
                    while (!CancellationTokenSource.Token.IsCancellationRequested)
                    {
                        SleepTime = null;
                        int page = 1;
                        DateTime today = DateTime.Now;
                        DateTime targetDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                        targetDate = targetDate.AddDays(-dayDifference);
                        logger.Information($"Servis başladılır. {targetDate.ToString()} tarixinədək elanları çəkəcək. Bitdikdən {repeatEvery} dəqiqə sonra yenidən işə düşəcək");
                        await YeniEmlakHelper.InitializeChromiumAsync();
                        while (!CancellationTokenSource.IsCancellationRequested)
                        {
                            TryAgain:
                            var htmlDocument = await YeniEmlakHelper.GetDocument(YeniEmlakConstants.BaseSearchUrl.Replace("XXXPAGEXXX", page.ToString()));
                            var advItems = YeniEmlakHelper.GetAdvItemNodes(htmlDocument);
                            if (advItems is null)
                                goto TryAgain;
                            var oldContentCount = 0;
                            var row = 1;
                            foreach (var item in advItems)
                            {
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var mainInfos = YeniEmlakHelper.GetMainInfosFromAdvItemNode(item);
                                var itemId = mainInfos.Item1;
                                var itemDate = DateTime.ParseExact(mainInfos.Item2, "dd.MM.yyyy", null);
                                var itemLink = mainInfos.Item3;
                                logger.Information($"{row++}/{advItems.Count} ({page} səhifə) {itemId} - {itemLink} prosess başladıldı.");
                                if (itemDate < targetDate)
                                {
                                    oldContentCount++;
                                    continue;
                                }
                                if (databaseService.FindById(int.Parse(itemId)) is not null)
                                {
                                    logger.Information($"{itemId} bazada tapıldı. Növbəti elana keçid edilir.");
                                    continue;
                                }
                                var detailHtmlDocumentString = await YeniEmlakHelper.GetDetailPageAsync(itemLink);
                                var detailHtmlDocument = new HtmlAgilityPack.HtmlDocument();
                                detailHtmlDocument.LoadHtml(detailHtmlDocumentString);
                                var property = YeniEmlakHelper.GetPropertyFromDocument(detailHtmlDocument);
                                property.Id = int.Parse(itemId);
                                property.AdvLink = itemLink;
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                databaseService.InsertRecord(property);
                                Progress++; 
                                logger.Information($"Bazaya əlavə edildi.");
                                await Task.Delay(700, CancellationTokenSource.Token);
                            }
                            page++;
                            if (oldContentCount == advItems.Count)
                                break;
                            await Task.Delay(1500, CancellationTokenSource.Token);
                        }
                        SleepTime = DateTime.Now;
                        logger.Information($"Elanlar limit tarixinə çatdı. Axtarış sonlanır. Növbəti axtarış {repeatEvery} dəqiqə sonra olacaq.");
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), CancellationTokenSource.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    logger.Information("Servise dayandırıldı.");
                }
                catch (Exception ex)
                {
                    logger.Error($"Servisdə xəta baş verdi: {ex.Message}");
                }
                finally
                {
                    IsRunning = false;
                    SleepTime = null;
                    Progress = 0;
                    RepeatEvery = 0;
                    CancellationTokenSource.Dispose();
                }
            });
        }

        public void Stop()
        {
            if (!IsRunning)
                return;
            CancellationTokenSource?.Cancel();
        }
    }
}
