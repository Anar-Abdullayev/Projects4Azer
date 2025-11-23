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
                                var mainInfos = YeniEmlakHelper.GetMainInfosFromAdvItemNode(item);
                                var itemId = mainInfos.Item1;
                                var itemDate = DateTime.ParseExact(mainInfos.Item2, "dd.MM.yyyy", null);
                                var itemLink = mainInfos.Item3;

                                logger.Information($"{row++}/{advItems.Count}({page} page) Starting to process id: {itemId}\n{itemLink}");
                                if (itemDate < targetDate)
                                {
                                    logger.Information($"Found old content. Moving to next");
                                    oldContentCount++;
                                    continue;
                                }
                                if (databaseService.FindById(int.Parse(itemId)) is not null)
                                {
                                    logger.Information("This item exists");
                                    continue;
                                }
                                var detailHtmlDocumentString = await YeniEmlakHelper.GetDetailPageAsync(itemLink);
                                var detailHtmlDocument = new HtmlAgilityPack.HtmlDocument();
                                detailHtmlDocument.LoadHtml(detailHtmlDocumentString);
                                var property = YeniEmlakHelper.GetPropertyFromDocument(detailHtmlDocument);
                                property.Id = int.Parse(itemId);
                                property.AdvLink = itemLink;
                                databaseService.InsertRecord(property);
                                Progress++;
                                logger.Information($"Item Id with {item.Id} inserted successfully");
                                await Task.Delay(700, CancellationTokenSource.Token);
                            }
                            page++;
                            if (oldContentCount == advItems.Count)
                                break;
                            await Task.Delay(1500, CancellationTokenSource.Token);
                        }
                        logger.Information("Set to waiting");
                        SleepTime = DateTime.Now;
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), CancellationTokenSource.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    logger.Information("Service stopped");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
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
