using HtmlAgilityPack;
using Serilog;
using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Bots.Tap.Consts;
using UniversalDataCatcher.Server.Bots.Tap.Helpers;
using UniversalDataCatcher.Server.Helpers;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Bots.Tap.Services
{
    public class TapAzService : BotService
    {
        private TapazMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;

        public TapAzService(TapazMSSqlDatabaseService _databaseService)
        {
            databaseService = _databaseService;
            logger = LoggerHelper.GetLoggerConfiguration(nameof(TapAzService));
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
                var tapazHelper = new TapazHelper();
                try
                {

                    while (!CancellationTokenSource.IsCancellationRequested)
                    {
                        SleepTime = null;
                        bool continueSearch = true;
                        int page = 1;
                        DateTime targetDate = DateTime.Now.AddDays(-dayDifference);
                        var tillDateString = FormatHelper.FormatAzeriDate(targetDate);
                        var formattedDates = FormatHelper.GetFormattedDatesUntil(targetDate);
                        string nextUrl = "/elanlar/dasinmaz-emlak";
                        while (!CancellationTokenSource.IsCancellationRequested && continueSearch)
                        {
                            var htmlContent = await tapazHelper.GetPage(nextUrl);
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(htmlContent);
                            nextUrl = DocumentHelper.GetNextPageUrl(doc);
                            if (htmlContent != null)
                            {
                                var propertyNodes = tapazHelper.GetPropertiesFromContent(doc, formattedDates, ref continueSearch);
                                if (propertyNodes != null)
                                {
                                    int row = 1;
                                    foreach (var propertyNode in propertyNodes)
                                    {
                                        CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        logger.Information($"{row++}/{propertyNodes.Count} ({page} page)");
                                        var existingRecord = databaseService.FindById(int.Parse(propertyNode.Item1));
                                        if (existingRecord != null)
                                        {
                                            logger.Information($"ID-si {propertyNode.Item1} olan elan oxunulub. Ötürülür...");
                                            logger.Information($"ID-si {propertyNode.Item1} üçün saytın URL-i: {propertyNode.Item2}");
                                            continue;
                                        }
                                        var detailHtml = await tapazHelper.GetPage(propertyNode.Item2);
                                        if (detailHtml == null)
                                            continue;
                                        var property = await tapazHelper.GetPropertyFromRawHTML(detailHtml, propertyNode.Item1);
                                        property.Id = int.Parse(propertyNode.Item1);
                                        property.AdvLink = Constants.BaseUrl + propertyNode.Item2;
                                        property.CreatedAt = FormatHelper.ParseAzeriDateWithTime(propertyNode.Item3);
                                        CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                        databaseService.InsertRecord(property);
                                        logger.Information("Yeni elan tapıldı və məlumat bazasına əlavə edildi:");
                                        Progress++;
                                        await Task.Delay(TimeSpan.FromSeconds(0.5), CancellationTokenSource.Token);
                                    }
                                }
                            }
                            page++;
                            await Task.Delay(TimeSpan.FromSeconds(0.5), CancellationTokenSource.Token);
                        }
                        logger.Information($"Gözləmə rejimində... Növbəti yoxlama {repeatEvery} dəqiqədən sonra baş tutacaq.");
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
                    logger.Error(ex.ToString());
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
