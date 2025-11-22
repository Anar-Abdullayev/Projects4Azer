using UniversalDataCatcher.Server.Bots.YeniEmlak.Helpers;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Services;
using UniversalDataCatcher.Server.Bots.YeniEmlak.StaticConstants;

namespace UniversalDataCatcher.Server.Bots.Emlak.Services
{
    public class EmlakService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;
        private EmlakMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;
        public EmlakService(EmlakMSSqlDatabaseService _databaseService, Serilog.ILogger _logger)
        {
            databaseService = _databaseService;
            logger = _logger.ForContext("ServiceName", nameof(YeniEmlakService));
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
                    logger.Information("Service stopped");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
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
            _progress = 0;
        }
    }
}
