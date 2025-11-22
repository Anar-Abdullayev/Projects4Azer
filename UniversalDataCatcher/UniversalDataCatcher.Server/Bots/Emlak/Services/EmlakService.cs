using UniversalDataCatcher.Server.Bots.Emlak.Helpers;
using UniversalDataCatcher.Server.Bots.Emlak.StaticConstants;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Helpers;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Services;
using UniversalDataCatcher.Server.Bots.YeniEmlak.StaticConstants;

namespace UniversalDataCatcher.Server.Bots.Emlak.Services
{
    public class EmlakService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;
        private EmlakMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;
        public EmlakService(EmlakMSSqlDatabaseService _databaseService, Serilog.ILogger _logger)
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
                        while (!_cts.IsCancellationRequested)
                        {
                            TryAgain:
                            var advItems = await EmlakHelper.GetAdvItemNodes(EmlakConstants.BaseSearchUrl.Replace("XXXPAGEXXX", page.ToString()));
                            if (advItems == null)
                            {
                                logger.Error("AdvItems is null. Retrying in 3 minutes");
                                await Task.Delay(TimeSpan.FromMinutes(3), _cts.Token);
                                goto TryAgain;
                            }
                            var row = 1;
                            var oldContentCount = 0;
                            foreach (var item in advItems)
                            {
                                _cts.Token.ThrowIfCancellationRequested();
                                logger.Information($"{row}/{advItems.Count} ({page} page) Starting process");
                                var initialInfo = EmlakHelper.GetInitialInfosFromNode(item);
                                var id = initialInfo.Item1;
                                var advLink = EmlakConstants.HostUrl+initialInfo.Item2;
                                logger.Information($"Processing link: {advLink}");
                                if (databaseService.FindById(int.Parse(id)) is not null)
                                {
                                    logger.Information("Item exists in database. Moving to next");
                                    continue;
                                }
                                var detailedItemDocument = await EmlakHelper.GetPageDocumentAsync(advLink);
                                if (!EmlakHelper.IsValidDetailPage(detailedItemDocument))
                                {
                                    logger.Error("Page is not found");
                                    logger.Error($"Page link: {advLink}");
                                    continue;
                                }
                                var property = EmlakHelper.GetProperty(detailedItemDocument);
                                property.Id = int.Parse(id);
                                property.AdvLink = advLink;

                                if (!item.GetAttributeValue("class", "").Contains("pinned"))
                                {
                                    if (property.CreatedAt < targetDate)
                                    {
                                        logger.Information("Old advertisement found moving to next");
                                        oldContentCount++;
                                        continue;
                                    }
                                }
                                databaseService.InsertRecord(property);
                                logger.Information($"{id} has been inserted successfully");
                                row++;
                                await Task.Delay(700, _cts.Token);
                            }
                            if (oldContentCount == advItems.Count)
                            {
                                logger.Information($"{oldContentCount} out of {advItems.Count} found as old. Ending search. Service will restart after {repeatEvery} minutes");
                                break;
                            }
                            page++;
                        }
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
            _progress = 0;
        }
    }
}
