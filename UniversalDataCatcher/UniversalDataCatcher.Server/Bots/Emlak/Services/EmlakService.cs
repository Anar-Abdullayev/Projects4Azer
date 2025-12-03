using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;
using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Bots.Emlak.Helpers;
using UniversalDataCatcher.Server.Bots.Emlak.StaticConstants;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Helpers;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Services;
using UniversalDataCatcher.Server.Bots.YeniEmlak.StaticConstants;
using UniversalDataCatcher.Server.Helpers;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Bots.Emlak.Services
{
    public class EmlakService : BotService
    {
        private EmlakMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;
        public EmlakService(EmlakMSSqlDatabaseService _databaseService)
        {
            databaseService = _databaseService;
            logger = LoggerHelper.GetLoggerConfiguration(nameof(EmlakService));
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
                        while (!CancellationTokenSource.IsCancellationRequested)
                        {
                            TryAgain:
                            var advItems = await EmlakHelper.GetAdvItemNodes(EmlakConstants.BaseSearchUrl.Replace("XXXPAGEXXX", page.ToString()));
                            if (advItems == null)
                            {
                                logger.Error("AdvItems is null. Retrying in 25 seconds");
                                await Task.Delay(TimeSpan.FromSeconds(25), CancellationTokenSource.Token);
                                goto TryAgain;
                            }
                            var row = 1;
                            var oldContentCount = 0;
                            foreach (var item in advItems)
                            {
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                logger.Information($"{row++}/{advItems.Count} ({page} səhifə) prosess başladıldı.");
                                var initialInfo = EmlakHelper.GetInitialInfosFromNode(item);
                                var id = initialInfo.Item1;
                                var advLink = EmlakConstants.HostUrl + initialInfo.Item2;
                                if (databaseService.FindById(int.Parse(id)) is not null)
                                {
                                    logger.Information($"{id} - {advLink} bazada tapıldı. Növbəti elana keçid edilir.");
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
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                databaseService.InsertRecord(property);
                                logger.Information($"Bazaya əlavə edildi.");
                                Progress++;
                                await Task.Delay(700, CancellationTokenSource.Token);
                            }
                            if (oldContentCount == advItems.Count)
                                break;
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
