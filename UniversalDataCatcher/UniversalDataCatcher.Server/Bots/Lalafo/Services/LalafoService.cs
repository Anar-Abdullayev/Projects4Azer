using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
                            var itemPosition = 1;
                            var items = await LalafoHelper.FetchApiPageAsync(cookies, page);
                            var outDateCount = 0;
                            foreach (var item in items)
                            {
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                logger.Information($"{itemPosition++}/{items.Count} ({page} səhifə) {item.Id} - {item.Url} prosess başladıldı.");
                                if (databaseService.FindById(item.Id) != null)
                                {
                                    logger.Information($"{item.Id} bazada tapıldı. Növbəti elana keçid edilir.");
                                    continue;
                                }
                                var createdDate = DateTimeOffset.FromUnixTimeSeconds(item.CreatedTime);
                                if (createdDate < targetDate)
                                {
                                    outDateCount++;
                                    continue;
                                }

                                var propertyDetails = await LalafoHelper.FetchDetailsPageAsync(cookies, item.Id);
                                propertyDetails.Ad_Label = item.Ad_Label;
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                databaseService.InsertRecord(propertyDetails);
                                logger.Information($"Bazaya əlavə edildi.");
                                Progress++;
                                await Task.Delay(1000, CancellationTokenSource.Token);
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                            }
                            if (outDateCount == items.Count)
                            {
                                break;
                            }
                            page++;
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
