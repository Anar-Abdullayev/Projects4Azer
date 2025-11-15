using UniversalDataCatcher.Server.Bots.Lalafo.Helpers;
using UniversalDataCatcher.Server.Helpers;

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
                var consoleHelper = new ConsoleHelper("LALAFO");
                var databaseService = new LalafoMSSqlDatabaseService();
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        bool continueSearch = true;
                        var limitdate = DateTime.Now.AddDays(-dayDifference);
                        var targetDate = new DateTime(limitdate.Year, limitdate.Month, limitdate.Day, 0, 0, 0);
                        var cookies = await LalafoHelper.GetCookiesAsync();
                        var page = 1;
                        consoleHelper.PrintText($"Servis başladılır. {targetDate.ToString()} tarixinədək elanları çəkəcək. Bitdikdən {repeatEvery} dəqiqə sonra yenidən işə düşəcək");
                        while (!_cts.IsCancellationRequested && continueSearch)
                        {
                            var itemPosition = 0;
                            var items = await LalafoHelper.FetchApiPageAsync(cookies, page);
                            var outDateCount = 0;
                            foreach (var item in items)
                            {
                                consoleHelper.PrintProgress(itemPosition++, items.Count, page);
                                if (databaseService.FindById(item.Id) != null)
                                {
                                    consoleHelper.PrintText($"{item.Id} bazada tapıldı. Növbəti elana keçid edilir.");
                                    consoleHelper.PrintText($"{item.Id} - {item.Url}");
                                    continue;
                                }
                                var createdDate = DateTimeOffset.FromUnixTimeSeconds(item.CreatedTime);
                                if (createdDate < targetDate)
                                {
                                    consoleHelper.PrintText($"Id ({item.Id}) {targetDate.ToString()} tarixindən köhnədir. Növbətinə keçid edilir.");
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
                                consoleHelper.PrintText($"Elanlar limit tarixinə çatdı. Axtarış sonlanır. Növbəti axtarış {repeatEvery} dəqiqə sonra olacaq.");
                                break;
                            }
                            page++;
                        }
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), _cts.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    consoleHelper.PrintText("Servis dayandırıldı.");
                }
                catch (Exception ex)
                {
                    consoleHelper.PrintText(ex.ToString());
                }
                finally
                {
                    _isRunning = false;
                    _progress = 0;
                    _cts.Dispose();
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
