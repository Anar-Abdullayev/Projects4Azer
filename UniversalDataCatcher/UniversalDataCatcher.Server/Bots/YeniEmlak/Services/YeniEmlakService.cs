using Microsoft.Playwright;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Helpers;
using UniversalDataCatcher.Server.Bots.YeniEmlak.StaticConstants;

namespace UniversalDataCatcher.Server.Bots.YeniEmlak.Services
{
    public class YeniEmlakService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;
        private YeniemlakMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;
        public YeniEmlakService(YeniemlakMSSqlDatabaseService _databaseService, Serilog.ILogger _logger)
        {
            databaseService = _databaseService;
            logger = _logger.ForContext("ServiceName", nameof(YeniEmlakService));
        }

        public void Start(int dayDifference, int repeatEvery)
        {
            if (_isRunning)
                return;
            _cts = new CancellationTokenSource();
            _isRunning = true;
            _progress = 0;

            Task.Run(async () =>
            {
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        int page = 1;
                        DateTime today = DateTime.Now;
                        DateTime targetDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                        targetDate = targetDate.AddDays(-dayDifference);
                        await YeniEmlakHelper.InitializeChromiumAsync();
                        while (!_cts.IsCancellationRequested)
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
                                logger.Information($"Item Id with {item.Id} inserted successfully");
                                await Task.Delay(700, _cts.Token);
                            }
                            page++;
                            if (oldContentCount == advItems.Count)
                                break;
                            await Task.Delay(1500, _cts.Token);
                        }
                        logger.Information("Set to waiting");
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), _cts.Token);
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
                    _isRunning = false;
                    _progress = 0;
                    _cts.Dispose();
                }
            });
        }

        public void Stop()
        {
            if (!_isRunning)
                return;
            _cts.Cancel();
            _isRunning = false;
        }
    }
}
