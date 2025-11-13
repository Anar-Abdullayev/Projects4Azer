using UniversalDataCatcher.Server.Services.Arenda.Helpers;

namespace UniversalDataCatcher.Server.Services.Arenda.Services
{
    public static class ArendaService
    {
        private static CancellationTokenSource _cts;
        private static bool _isRunning = false;
        private static int _progress = 0;
        public static bool IsRunning => _isRunning;
        public static int Progress => _progress;

        public static void Start(int dayDifference)
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
                    bool continueSearch = true;
                    var databaseService = new ArendaMSSqlDatabaseService();
                    int page = 1;
                    DateTime targetDate = DateTime.Now.AddDays(-dayDifference);
                    var tillDateString = FormatHelper.FormatAzeriDate(targetDate);
                    var formattedDates = FormatHelper.GetFormattedDatesUntil(targetDate);
                    Console.WriteLine($"Təsdiqlənmiş gün: {dayDifference} gün öncəyədək ({tillDateString} daxil deyil).");
                    Console.WriteLine($"Başlayır...");
                    while (!_cts.Token.IsCancellationRequested && continueSearch)
                    {
                        var htmlContent = await ArendaHelper.GetPage(page);
                        if (htmlContent != null)
                        {
                            var propertyNodes = ArendaHelper.GetPropertiesFromContent(htmlContent, formattedDates, ref continueSearch);
                            if (propertyNodes != null)
                            {
                                Console.WriteLine($"Səhifə - {page}");
                                int row = 1;
                                foreach (var propertyNode in propertyNodes)
                                {
                                    _cts.Token.ThrowIfCancellationRequested();
                                    Console.Write($"{row++}/{page} - ");
                                    var existingRecord = databaseService.FindById(propertyNode.Item1);
                                    if (existingRecord != null)
                                    {
                                        Console.WriteLine($"ID-si {propertyNode.Item1} olan elan oxunulub. Ötürülür...");
                                        continue;
                                    }

                                    var detailHtml = await ArendaHelper.GetPropertyDetailPage(propertyNode.Item2);
                                    if (detailHtml == null)
                                        continue;
                                    var property = ArendaHelper.GetPropertyFromRawHTML(detailHtml);
                                    property.Id = propertyNode.Item1;
                                    databaseService.InsertRecord(property);
                                    Console.WriteLine("--------------------------------");
                                    Console.WriteLine($"Yeni elan tapıldı");
                                    Console.WriteLine($"Elanın linki: {propertyNode.Item2}");
                                    Console.WriteLine($"Elanın tarixi: {propertyNode.Item3}");
                                    Console.WriteLine("--------------------------------");
                                    await Task.Delay(TimeSpan.FromSeconds(2), _cts.Token);
                                }
                            }
                        }
                        page++;
                        await Task.Delay(TimeSpan.FromSeconds(1), _cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("ERROR: Arenda Bot Service has been cancelled!");
                }
                finally
                {
                    _isRunning = false;
                    Console.WriteLine("Arenda Bot Service stopped.");
                }
            });
        }

        public static void Stop()
        {
            if (!_isRunning)
                return;
            _cts.Cancel();
            _isRunning = false;
        }
    }
}
