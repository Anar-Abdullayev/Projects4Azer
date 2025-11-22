using UniversalDataCatcher.Server.Bots.Lalafo.Helpers;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Services
{
    public class LalafoService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;
        private LalafoMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;

        public LalafoService(LalafoMSSqlDatabaseService _databaseService, Serilog.ILogger _logger)
        {
            databaseService = _databaseService;
            logger = _logger.ForContext("ServiceName", nameof(LalafoService));
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
                        bool continueSearch = true;
                        var limitdate = DateTime.Now.AddDays(-dayDifference);
                        var targetDate = new DateTime(limitdate.Year, limitdate.Month, limitdate.Day, 0, 0, 0);
                        var cookies = await LalafoHelper.GetCookiesAsync();
                        var page = 1;
                        logger.Information($"Servis başladılır. {targetDate.ToString()} tarixinədək elanları çəkəcək. Bitdikdən {repeatEvery} dəqiqə sonra yenidən işə düşəcək");
                        while (!_cts.IsCancellationRequested && continueSearch)
                        {
                            var itemPosition = 0;
                            var items = await LalafoHelper.FetchApiPageAsync(cookies, page);
                            var outDateCount = 0;
                            foreach (var item in items)
                            {
                                logger.Information($"{itemPosition++}/{items.Count} ({page} page)");
                                if (databaseService.FindById(item.Id) != null)
                                {
                                    logger.Information($"{item.Id} bazada tapıldı. Növbəti elana keçid edilir.");
                                    logger.Information($"{item.Id} - {item.Url}");
                                    continue;
                                }
                                var createdDate = DateTimeOffset.FromUnixTimeSeconds(item.CreatedTime);
                                if (createdDate < targetDate)
                                {
                                    logger.Information($"Id ({item.Id}) {targetDate.ToString()} tarixindən köhnədir. Növbətinə keçid edilir.");
                                    outDateCount++;
                                    continue;
                                }
                                var propertyDetails = await LalafoHelper.FetchDetailsPageAsync(cookies, item.Id);
                                propertyDetails.Ad_Label = item.Ad_Label;
                                databaseService.InsertRecord(propertyDetails);
                                _progress++;
                                await Task.Delay(1000, _cts.Token);
                                _cts.Token.ThrowIfCancellationRequested();
                            }
                            if (outDateCount == items.Count)
                            {
                                logger.Information($"Elanlar limit tarixinə çatdı. Axtarış sonlanır. Növbəti axtarış {repeatEvery} dəqiqə sonra olacaq.");
                                break;
                            }
                            page++;
                        }
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), _cts.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    logger.Information("Servis dayandırıldı.");
                }
                catch (Exception ex)
                {
                    logger.Information(ex.ToString());
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
