using UniversalDataCatcher.Server.Bots.Lalafo.Helpers;
using UniversalDataCatcher.Server.Bots.Lalafo.Services;
using UniversalDataCatcher.Server.Helpers;

namespace UniversalDataCatcher.Server.Bots.EvTen.Services
{
    public static class EvTenService
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
                        while (!_cts.IsCancellationRequested)
                        {

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
