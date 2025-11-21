using System.Text.Json;
using System.Text.RegularExpressions;
using UniversalDataCatcher.Server.Bots.EvTen.Helpers;
using UniversalDataCatcher.Server.Bots.EvTen.Models;
using UniversalDataCatcher.Server.Bots.EvTen.StaticConstants;
using UniversalDataCatcher.Server.Helpers;
using Serilog;

namespace UniversalDataCatcher.Server.Bots.EvTen.Services
{
    public class EvTenService
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;
        private int _progress = 0;
        public bool IsRunning => _isRunning;
        public int Progress => _progress;
        private EvTenMSSqlDatabaseService databaseService;
        private Serilog.ILogger logger;
        public EvTenService(EvTenMSSqlDatabaseService _databaseService, Serilog.ILogger _logger)
        {
            databaseService = _databaseService;
            logger = _logger.ForContext("ServiceName", nameof(EvTenService));
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
                        int page = 1;
                        DateTime today = DateTime.Now;
                        DateTime targetDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                        targetDate = targetDate.AddDays(-dayDifference);
                        while (!_cts.IsCancellationRequested)
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
                                string cleaned = Regex.Unescape(objJson);

                                // Optional: trim surrounding quotes if objJson came quoted
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
                                if (databaseService.FindById(item.Id) is not null)
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
                                postingJson = EvTenHelper.FixJsonString(postingJson);
                                var detailedItem = JsonSerializer.Deserialize<EvTenPropertyDetails>(postingJson);
                                detailedItem.MainTitle = DocumentHelper.GetMainTitle(detailedHtmlString);
                                if (detailedItem.Description == "$3b")
                                    detailedItem.Description = DocumentHelper.GetDescriptionFromMergedString(merged);
                                detailedItem.HasIpoteka = DocumentHelper.HasIpotekaInfo(detailedHtmlString);
                                databaseService.InsertRecord(detailedItem);
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
                        await Task.Delay(TimeSpan.FromMinutes(repeatEvery), _cts.Token);
                    }

                }
                catch (OperationCanceledException)
                {
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
        }
    }
}
