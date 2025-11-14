using UniversalDataCatcher.Server.Bots.Lalafo.Helpers;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Services
{
    public static class LalafoService
    {
        private static CancellationTokenSource _cts;
        private static bool _isRunning = false;
        private static int _progress = 0;
        public static bool IsRunning => _isRunning;
        public static int Progress => _progress;

        public static void Start(int dayDifference, int repeatEvery)
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
                    var databaseService = new LalafoMSSqlDatabaseService();
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        bool continueSearch = true;
                        var limitdate = DateTime.Now.AddDays(-dayDifference);
                        var targetDate = new DateTime(limitdate.Year, limitdate.Month, limitdate.Day, 0, 0, 0);
                        var cookies = await LalafoHelper.GetCookiesAsync();
                        var page = 1;
                        while (!_cts.IsCancellationRequested && continueSearch)
                        {
                            var items = await LalafoHelper.FetchApiPageAsync(cookies, page++);
                            var outDateCount = 0;
                            foreach (var item in items)
                            {
                                var propertyDetails = await LalafoHelper.FetchDetailsPageAsync(cookies, item.Id);
                                var createdDate = DateTimeOffset.FromUnixTimeSeconds(propertyDetails.CreatedTime);
                                if (createdDate < targetDate)
                                    outDateCount++;
                                propertyDetails.PrintDetails();
                                _progress++;
                                await Task.Delay(1000, _cts.Token);
                                _cts.Token.ThrowIfCancellationRequested();
                            }
                            if (outDateCount == items.Count)
                            {
                                Console.WriteLine("(LALAFOAZ): Items reached to limit date. Breaking the loop. Waiting for the next part");
                                break;
                            }
                        }
                        await Task.Delay(repeatEvery * 1000, _cts.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("(LALAFOAZ): ABORT: Lalafo Service has been cancelled");
                    _isRunning = false;
                    _progress = 0;
                    _cts.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.WriteLine("(LALAFOAZ): Lalafo Service has been stopped");
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
