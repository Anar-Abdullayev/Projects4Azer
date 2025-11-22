using HtmlAgilityPack;
using UniversalDataCatcher.Server.Bots.Tap.Consts;
using UniversalDataCatcher.Server.Bots.Tap.Helpers;

namespace UniversalDataCatcher.Server.Bots.Tap.Services
{
    public class TapAzService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;

        private TapazMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;

        public TapAzService(TapazMSSqlDatabaseService _databaseService, Serilog.ILogger _logger)
        {
            databaseService = _databaseService;
            logger = _logger.ForContext("ServiceName", nameof(TapAzService));
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
                var tapazHelper = new TapazHelper();
                try
                {

                    while (!_cts.IsCancellationRequested)
                    {
                        bool continueSearch = true;
                        int page = 1;
                        DateTime targetDate = DateTime.Now.AddDays(-dayDifference);
                        var tillDateString = FormatHelper.FormatAzeriDate(targetDate);
                        var formattedDates = FormatHelper.GetFormattedDatesUntil(targetDate);
                        string nextUrl = "/elanlar/dasinmaz-emlak";
                        while (!_cts.IsCancellationRequested && continueSearch)
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
                                        logger.Information($"{row}/{propertyNodes.Count} ({page} page)");
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
                                        databaseService.InsertRecord(property);
                                        logger.Information("Yeni elan tapıldı və məlumat bazasına əlavə edildi:");
                                        row++;
                                        await Task.Delay(TimeSpan.FromSeconds(0.5), _cts.Token);
                                    }
                                }
                            }
                            page++;
                            await Task.Delay(TimeSpan.FromSeconds(0.5),_cts.Token);
                        }
                        logger.Information($"Gözləmə rejimində... Növbəti yoxlama {repeatEvery} dəqiqədən sonra baş tutacaq.");
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
