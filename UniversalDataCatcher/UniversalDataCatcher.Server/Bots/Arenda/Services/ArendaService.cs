using UniversalDataCatcher.Server.Services.Arenda.Helpers;

namespace UniversalDataCatcher.Server.Services.Arenda.Services
{
    public class ArendaService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;
        private ArendaMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;
        public ArendaService(ArendaMSSqlDatabaseService _databaseService, Serilog.ILogger _logger)
        {
            databaseService = _databaseService;
            logger = _logger.ForContext("ServiceName", nameof(ArendaService));
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
                databaseService = new ArendaMSSqlDatabaseService();
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        bool continueSearch = true;
                        int page = 1;
                        DateTime targetDate = DateTime.Now.AddDays(-dayDifference);
                        var tillDateString = FormatHelper.FormatAzeriDate(targetDate);
                        var formattedDates = FormatHelper.GetFormattedDatesUntil(targetDate);
                        logger.Information($"Servis başladılır. {targetDate.ToString()} tarixinədək elanları çəkəcək. Bitdikdən {repeatEvery} dəqiqə sonra yenidən işə düşəcək");
                        while (!_cts.Token.IsCancellationRequested && continueSearch)
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
                                        _cts.Token.ThrowIfCancellationRequested();
                                        logger.Information($"{row}/{propertyNodes.Count} ({page} page) Starting process");
                                        var existingRecord = databaseService.FindById(int.Parse(propertyNode.Item1.Replace("elan_", "")));
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
                                        _progress++;
                                        await Task.Delay(TimeSpan.FromSeconds(1), _cts.Token);
                                    }
                                }
                            }
                            page++;
                            await Task.Delay(TimeSpan.FromSeconds(0.5), _cts.Token);
                        }
                        logger.Information($"Elanlar limit tarixinə çatdı. Axtarış sonlanır. Növbəti axtarış {repeatEvery} dəqiqə sonra olacaq.");
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), _cts.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    logger.Information("ERROR: Arenda Bot Service has been cancelled!");
                }
                catch (Exception ex)
                {
                    logger.Information($"ERROR: An unexpected error occurred in Arenda Bot Service: {ex.Message}");
                }
                finally
                {
                    _isRunning = false;
                    _cts.Dispose();
                    logger.Information("Servis dayandırıldı.");
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
