using Serilog;
using System.Text.Json;
using System.Text.RegularExpressions;
using UniversalDataCatcher.Server.Abstracts;
using UniversalDataCatcher.Server.Bots.EvTen.Helpers;
using UniversalDataCatcher.Server.Bots.EvTen.Models;
using UniversalDataCatcher.Server.Bots.EvTen.StaticConstants;
using UniversalDataCatcher.Server.Entities;
using UniversalDataCatcher.Server.Extentions;
using UniversalDataCatcher.Server.Helpers;
using UniversalDataCatcher.Server.Interfaces;

namespace UniversalDataCatcher.Server.Bots.EvTen.Services
{
    public class EvTenService : BotService
    {
        private EvTenMSSqlDatabaseService _databaseService;
        private Serilog.ILogger logger;
        public EvTenService(EvTenMSSqlDatabaseService databaseService)
        {
            _databaseService = databaseService;
            logger = LoggerHelper.GetLoggerConfiguration(nameof(EvTenService));
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
                try
                {
                    while (!CancellationTokenSource.Token.IsCancellationRequested)
                    {
                        SleepTime = null;
                        int page = 1;
                        DateTime today = DateTime.Now;
                        DateTime targetDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                        targetDate = targetDate.AddDays(-dayDifference);
                        while (!CancellationTokenSource.IsCancellationRequested)
                        {
                            logger.Information(page.ToString());
                            var htmlString = await EvTenHelper.GetPageAsync(EvTenConstants.EvTenBaseUrl.Replace("XXPAGEXX", page.ToString()));
                            string merged = EvTenHelper.ExtractNextFData(htmlString);
                            var dict = EvTenHelper.ParseKeyValueMap(merged);
                            List<string> objects = EvTenHelper.BuildFullObjects(dict);
                            int oldContentCount = 0;
                            int row = 1;
                            foreach (var objJson in objects)
                            {
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                logger.Information($"{row++}/{objects.Count} ({page} page) Starting process");
                                string cleaned = Regex.Unescape(objJson);
                                if (cleaned.Contains(""))
                                    if (cleaned.StartsWith("\"") && cleaned.EndsWith("\""))
                                    {
                                        cleaned = cleaned.Substring(1, cleaned.Length - 2);
                                    }
                                cleaned = Regex.Replace(
                                    cleaned,
                                    "\"images\":\"(\\[.*?\\])\"",
                                    "\"images\":$1"
                                );
                                cleaned = Regex.Replace(
                                    cleaned,
                                    "\"subway_station\":\"(\\{.*?\\})\"",
                                    "\"subway_station\":$1"
                                );
                                EvTenProperty item = null;
                                item = JsonSerializer.Deserialize<EvTenProperty>(cleaned)!;
                                item.RenewedAt = item.RenewedAt.AddHours(4);

                                if (item.RenewedAt < targetDate)
                                {
                                    logger.Information($"Old content ({item.Id}) found, moving to next content");
                                    oldContentCount++;
                                    continue;
                                }
                                if (_databaseService.FindById(item.Id) is not null)
                                {
                                    logger.Information($"Recording with this id ({item.Id}) exists");
                                    continue;
                                }
                                logger.Information($"Starting to process id:({item.Id})");
                                var detailedHtmlString = await EvTenHelper.GetPageAsync(EvTenConstants.EvTenItemBaseUrl + item.Id);
                                merged = EvTenHelper.ExtractNextFData(detailedHtmlString);
                                dict = EvTenHelper.ParseKeyValueMap2(merged);
                                var postingKey = EvTenHelper.ExtractPostingKey(merged);
                                if (!dict.TryGetValue(postingKey.Replace("$", ""), out string postingJson))
                                    throw new Exception($"{postingKey} not found in dictionary");
                                postingJson = EvTenHelper.ReplacePlaceholders(postingJson, dict);
                                postingJson = Regex.Unescape(postingJson);
                                var detailedItem = JsonSerializer.Deserialize<EvTenPropertyDetails>(postingJson);
                                detailedItem.MainTitle = DocumentHelper.GetMainTitle(detailedHtmlString);
                                if (detailedItem.Description == "$3b")
                                    detailedItem.Description = DocumentHelper.GetDescriptionFromMergedString(merged);
                                detailedItem.HasIpoteka = DocumentHelper.HasIpotekaInfo(detailedHtmlString);
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                _databaseService.InsertRecord(detailedItem);
                                Progress++;
                                logger.Information($"Advertisement Id: {detailedItem.Id} has been inserted successfully");
                            }
                            if (oldContentCount >= objects.Count)
                            {
                                logger.Information("Old content threshold reached, moving to next cycle.");
                                break;
                            }
                            page++;
                        }
                        logger.Information("Set to waiting");
                        SleepTime = DateTime.Now;
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), CancellationTokenSource.Token);
                    }

                }
                catch (OperationCanceledException)
                {
                    logger.Information("Service cancelled");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
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
