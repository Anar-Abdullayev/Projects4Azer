using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Bots.Lalafo.Helpers;
using UniversalDataCatcher.Server.Helpers;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Services
{
    public class LalafoService : BotService
    {
        private LalafoMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;

        public LalafoService(LalafoMSSqlDatabaseService _databaseService)
        {
            databaseService = _databaseService;
            logger = LoggerHelper.GetLoggerConfiguration(nameof(LalafoService));
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
                        bool continueSearch = true;
                        var limitdate = DateTime.Now.AddDays(-dayDifference);
                        var targetDate = new DateTime(limitdate.Year, limitdate.Month, limitdate.Day, 0, 0, 0);
                        var cookies = await LalafoHelper.GetCookiesAsync();
                        var page = 1;
                        logger.Information($"Servis başladılır. {targetDate.ToString()} tarixinədək elanları çəkəcək. Bitdikdən {repeatEvery} dəqiqə sonra yenidən işə düşəcək");
                        while (!CancellationTokenSource.IsCancellationRequested && continueSearch)
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
                                Progress++;
                                await Task.Delay(1000, CancellationTokenSource.Token);
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                            }
                            if (outDateCount == items.Count)
                            {
                                logger.Information($"Elanlar limit tarixinə çatdı. Axtarış sonlanır. Növbəti axtarış {repeatEvery} dəqiqə sonra olacaq.");
                                break;
                            }
                            page++;
                        }
                        SleepTime = DateTime.Now;
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), CancellationTokenSource.Token);
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
                    IsRunning = false;
                    SleepTime = null;
                    Progress = 0;
                    RepeatEvery = 0;
                    CancellationTokenSource.Dispose();
                    logger.Information("Servis dayandırıldı.");
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
