using Serilog;
using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Bots.Bina.Helpers;
using UniversalDataCatcher.Server.Bots.Bina.Models;
using UniversalDataCatcher.Server.Helpers;
using UniversalDataCatcher.Server.Services.Arenda.Services;

namespace UniversalDataCatcher.Server.Bots.Bina.Services
{
    public class BinaService : BotService
    {
        private BinaMSSqlDatabaseService database;
        private Serilog.ILogger logger;

        public BinaService(BinaMSSqlDatabaseService _database)
        {
            database = _database;
            logger = LoggerHelper.GetLoggerConfiguration(nameof(BinaService));
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
                var helper = new BinaAzContentHelper();
                try
                {
                    logger.Information("Servis başladıldı.");
                    while (!CancellationTokenSource.IsCancellationRequested)
                    {
                        SleepTime = null;
                        DateTime endDate = DateTime.Today.AddDays(-dayDifference);
                        bool continueSearch = true;
                        int page = 1;
                        await BinaAzHelper.StartInitialRun();
                        string? cursor = null;
                        Variables variables = new Variables() { first = 16, sort = "BUMPED_AT_DESC" };
                        if (cursor != null)
                        {
                            variables.cursor = cursor;
                        }
                        Extensions extensions = new Extensions() { persistedQuery = new PersistedQuery() { version = 1, sha256Hash = "872e9c694c34b6674514d48e9dcf1b46241d3d79f365ddf20d138f18e74554c5" } };
                        GraphqlQueryParams queryParams = new GraphqlQueryParams() { variables = variables, extensions = extensions, operationName = "SearchItems" };
                        while (!CancellationTokenSource.IsCancellationRequested && continueSearch)
                        {
                            var dataPage = await BinaAzHelper.GetData(queryParams);
                            if (dataPage is null)
                                return;
                            int currentItem = 1;
                            foreach (var item in dataPage.Data.ItemsConnection.Edges)
                            {
                                logger.Information($"{currentItem++}/{dataPage.Data.ItemsConnection.Edges.Count} ({page} page) Starting process");
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
                                Progress++;
                                await Task.Delay(500, CancellationTokenSource.Token);
                            }
                            page++;
                            await Task.Delay(1500, CancellationTokenSource.Token);
                        }
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
                    logger.Information(ex.ToString());
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
