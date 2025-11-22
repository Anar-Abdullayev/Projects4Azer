using UniversalDataCatcher.Server.Bots.Bina.Helpers;
using UniversalDataCatcher.Server.Bots.Bina.Models;

namespace UniversalDataCatcher.Server.Bots.Bina.Services
{
    public class BinaService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;
        private BinaMSSqlDatabaseService database;
        private Serilog.ILogger logger;

        public BinaService(BinaMSSqlDatabaseService _database, Serilog.ILogger _logger)
        {
            database = _database;
            logger = _logger.ForContext("ServiceName", nameof(BinaService));
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
                var helper = new BinaAzContentHelper();
                try
                {
                    logger.Information("Servis başladıldı.");
                    while (!_cts.IsCancellationRequested)
                    {
                        DateTime endDate = DateTime.Today.AddDays(-dayDifference);
                        bool continueSearch = true;
                        int page = 1;
                        int currentItem = 1;
                        await BinaAzHelper.StartInitialRun();
                        string? cursor = null;
                        Variables variables = new Variables() { first = 16, sort = "BUMPED_AT_DESC" };
                        if (cursor != null)
                        {
                            variables.cursor = cursor;
                        }
                        Extensions extensions = new Extensions() { persistedQuery = new PersistedQuery() { version = 1, sha256Hash = "872e9c694c34b6674514d48e9dcf1b46241d3d79f365ddf20d138f18e74554c5" } };
                        GraphqlQueryParams queryParams = new GraphqlQueryParams() { variables = variables, extensions = extensions, operationName = "SearchItems" };
                        while (!_cts.IsCancellationRequested && continueSearch)
                        {
                            var dataPage = await BinaAzHelper.GetData(queryParams);
                            if (dataPage is null)
                                return;

                            foreach (var item in dataPage.Data.ItemsConnection.Edges)
                            {
                                logger.Information($"{currentItem}/{dataPage.Data.ItemsConnection.Edges.Count} ({page} page) Starting process");
                                var property = item.Node.GetInitialProperty();
                                if (property.UpdatedTime < endDate)
                                {
                                    logger.Information("Data older than 2 days reached, stopping the process.");
                                    continueSearch = false;
                                    break;
                                }
                                var existingRecord = database.FindById(property.Id);
                                if (existingRecord is not null)
                                {
                                    logger.Information($"Recording with this id ({property.Id}) exists");
                                    continue;
                                }
                                var htmlString = await helper.GetPage($"/items/{property.Id}");
                                if (string.IsNullOrEmpty(htmlString))
                                    throw new Exception("Html Content returned null or empty");
                                var contentProperty = await helper.GetPropertyFromRawHTML(htmlString, property);
                                database.InsertRecord(contentProperty);
                                _progress++;
                                await Task.Delay(500, _cts.Token);
                            }
                            currentItem = 1;
                            page++;
                            await Task.Delay(1500, _cts.Token);
                        }
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
            _progress = 0;
        }
    }
}
