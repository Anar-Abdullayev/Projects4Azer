using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;
using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Bots.Bina.Constants;
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
                        logger.Information($"Servis başladılır. {endDate.ToString()} tarixinədək elanları çəkəcək. Bitdikdən {repeatEvery} dəqiqə sonra yenidən işə düşəcək");
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
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                var property = item.Node.GetInitialProperty();
                                logger.Information($"{currentItem++}/{dataPage.Data.ItemsConnection.Edges.Count} ({page} səhifə) {property.Id} - {Constants.Constants.BaseUrl}/items/{property.Id} prosess başladıldı.");
                                if (property.UpdatedTime < endDate)
                                {
                                    continueSearch = false;
                                    break;
                                }
                                var existingRecord = database.FindById(property.Id);
                                if (existingRecord is not null)
                                {
                                    logger.Information($"{existingRecord.Id} bazada tapıldı. Növbəti elana keçid edilir.");
                                    continue;
                                }
                                var retryDetails = 0;
                                TryGettingDetails:
                                var htmlString = await helper.GetPage($"/items/{property.Id}");
                                if (string.IsNullOrEmpty(htmlString))
                                {
                                    retryDetails++;
                                    await Task.Delay(TimeSpan.FromSeconds(15), CancellationTokenSource.Token);
                                    if (retryDetails == 3)
                                    {
                                        logger.Error("Html content is null for property id: " + property.Id);
                                        logger.Error("Url: " + Constants.Constants.BaseUrl + "/items/" + property.Id);
                                        continue;
                                    }
                                    goto TryGettingDetails;
                                }
                                var jsonText = helper.GetItemDetails(htmlString);

                                //var contentProperty = await helper.GetPropertyFromRawHTML(htmlString, property);
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                //database.InsertRecord(contentProperty);
                                //logger.Information($"Bazaya əlavə edildi.");
                                Progress++;
                                await Task.Delay(500, CancellationTokenSource.Token);
                            }
                            page++;
                            await Task.Delay(1500, CancellationTokenSource.Token);
                        }
                        logger.Information($"Elanlar limit tarixinə çatdı. Axtarış sonlanır. Növbəti axtarış {repeatEvery} dəqiqə sonra olacaq.");
                        SleepTime = DateTime.Now;
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), CancellationTokenSource.Token);
                    }


                }
                catch (OperationCanceledException)
                {
                    logger.Information("Servise dayandırıldı.");
                }
                catch (Exception ex)
                {
                    logger.Error($"Servisdə xəta baş verdi: {ex.Message}");
                }
                finally
                {
                    IsRunning = false;
                    SleepTime = null;
                    Progress = 0;
                    RepeatEvery = 0;
                    CancellationTokenSource.Dispose();
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
