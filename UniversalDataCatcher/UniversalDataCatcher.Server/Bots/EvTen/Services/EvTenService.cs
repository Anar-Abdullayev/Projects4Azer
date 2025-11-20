using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using UniversalDataCatcher.Server.Bots.EvTen.Helpers;
using UniversalDataCatcher.Server.Bots.EvTen.Models;
using UniversalDataCatcher.Server.Bots.EvTen.StaticConstants;
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
                var consoleHelper = new ConsoleHelper("EV10");
                var databaseService = new EvTenMSSqlDatabaseService();
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
                            var htmlString = await EvTenHelper.GetPageAsync(EvTenConstants.EvTenBaseUrl.Replace("XXPAGEXX", page.ToString()));
                            string merged = EvTenHelper.ExtractNextFData(htmlString);
                            var dict = EvTenHelper.ParseKeyValueMap(merged);
                            List<string> objects = EvTenHelper.BuildFullObjects(dict);
                            int oldContentCount = 0;
                            int row = 1;
                            foreach (var objJson in objects)
                            {
                                consoleHelper.PrintProgress(row++, objects.Count, page);
                                string cleaned = objJson.Replace("\\", "");
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
                                    consoleHelper.PrintText($"Old content ({item.Id}) found, moving to next content.");
                                    oldContentCount++;
                                    continue;
                                }
                                if (databaseService.FindById(item.Id) is not null)
                                {
                                    consoleHelper.PrintText($"Recording with this id ({item.Id}) exists");
                                    continue;
                                }
                                var detailedHtmlString = await EvTenHelper.GetPageAsync(EvTenConstants.EvTenItemBaseUrl+item.Id);
                                merged = EvTenHelper.ExtractNextFData(detailedHtmlString);
                                dict = EvTenHelper.ParseKeyValueMap(merged);
                                var postingKey = EvTenHelper.ExtractPostingKey(merged);
                                if (!dict.TryGetValue(postingKey.Replace("$", ""), out string postingJson))
                                    throw new Exception($"{postingKey} not found in dictionary");
                                postingJson = EvTenHelper.ReplacePlaceholders(postingJson, dict);
                                postingJson = EvTenHelper.FixJsonString(postingJson);
                                var detailedItem = JsonSerializer.Deserialize<EvTenPropertyDetails>(postingJson);
                                detailedItem.MainTitle = DocumentHelper.GetMainTitle(detailedHtmlString);
                                if (detailedItem.Description == "$3b")
                                    detailedItem.Description = DocumentHelper.GetDescriptionFromMergedString(merged);
                                databaseService.InsertRecord(detailedItem);
                            }
                            if (oldContentCount >= objects.Count)
                            {
                                consoleHelper.PrintText("Old content threshold reached, moving to next cycle.");
                                break;
                            }
                            page++;
                        }
                        consoleHelper.PrintText($"Gözləmə rejiminə keçildi... {repeatEvery} dəqiqə sonra yenidən başlayacaq.");
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
