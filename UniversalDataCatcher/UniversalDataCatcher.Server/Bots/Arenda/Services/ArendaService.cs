using HtmlAgilityPack;
using Serilog;
using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Helpers;
using UniversalDataCatcher.Server.Services.Arenda.Helpers;

namespace UniversalDataCatcher.Server.Services.Arenda.Services
{
    public class ArendaService : BotService
    {
        private ArendaMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;

        public ArendaService(ArendaMSSqlDatabaseService _databaseService)
        {
            databaseService = _databaseService;
            logger = LoggerHelper.GetLoggerConfiguration(nameof(ArendaService));
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
                        int page = 1;
                        DateTime targetDate = DateTime.Now.AddDays(-dayDifference);
                        var tillDateString = FormatHelper.FormatAzeriDate(targetDate);
                        var formattedDates = FormatHelper.GetFormattedDatesUntil(targetDate);
                        logger.Information($"Servis başladılır. {targetDate.ToString()} tarixinədək elanları çəkəcək. Bitdikdən {repeatEvery} dəqiqə sonra yenidən işə düşəcək");
                        while (!CancellationTokenSource.Token.IsCancellationRequested && continueSearch)
                        {
                            var htmlContent = await ArendaHelper.GetPage(page);
                            if (htmlContent != null)
                            {
                                var propertyNodes = ArendaHelper.GetPropertiesFromContent(htmlContent, formattedDates, ref continueSearch);
                                if (propertyNodes != null)
                                {
                                    int row = 1;
                                    foreach (var propertyNode in propertyNodes)
                                    {
                                        CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        logger.Information($"{row++}/{propertyNodes.Count} ({page} page) Starting process");
                                        var existingRecord = databaseService.FindById(int.Parse(propertyNode.Item1.Replace("elan_", "")));
                                        //var existingRecord = databaseService.FindByElanLink(propertyNode.Item2);
                                        if (existingRecord != null)
                                        {
                                            logger.Information($"{existingRecord.Id} bazada tapıldı. Növbəti elana keçid edilir.");
                                            logger.Information($"{existingRecord.Id} - {propertyNode.Item2}");
                                            continue;
                                        }

                                        var detailHtml = await ArendaHelper.GetPropertyDetailPage(propertyNode.Item2);
                                        if (detailHtml == null)
                                            continue;
                                        var property = ArendaHelper.GetPropertyFromRawHTML(detailHtml);
                                        property.Id = propertyNode.Item1;
                                        property.Link = propertyNode.Item2;
                                        property.Created_At = FormatHelper.ParseAzeriDateWithTime(propertyNode.Item3);
                                        databaseService.InsertRecord(property);
                                        Progress++;
                                        await Task.Delay(TimeSpan.FromSeconds(1), CancellationTokenSource.Token);
                                    }
                                }
                            }
                            page++;
                            await Task.Delay(TimeSpan.FromSeconds(0.5), CancellationTokenSource.Token);
                        }
                        logger.Information($"Elanlar limit tarixinə çatdı. Axtarış sonlanır. Növbəti axtarış {repeatEvery} dəqiqə sonra olacaq.");
                        SleepTime = DateTime.Now;
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), CancellationTokenSource.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    logger.Information("ERROR: Arenda Bot Service has been cancelled!");
                }
                catch (Exception ex)
                {
                    logger.Error($"ERROR: An unexpected error occurred in Arenda Bot Service: {ex.Message}");
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
