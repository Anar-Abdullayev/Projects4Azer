using Microsoft.Playwright;
using System.Text.Json;
using System.Text.RegularExpressions;
using UniversalDataCatcher.Server.Bots.Lalafo.Models;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Helpers
{
    public static class LalafoHelper
    {
        private static readonly string BaseUrl = "https://lalafo.az/azerbaijan/nedvizhimost?sort_by=newest";
        private static readonly string ApiUrl = "https://lalafo.az/api/search/v3/feed/search";
        private static readonly string DetailApiUrl = "https://lalafo.az/api/search/v3/feed/details/";
        public static readonly string WebsiteUrl = "https://lalafo.az";
        private static readonly int ItemsPerPage = 20;
        private static readonly int CategoryId = 2029;
        private static readonly string SortBy = "newest";
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<Dictionary<string, string>> GetCookiesAsync()
        {
            var result = new Dictionary<string, string>();
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true, Timeout = 30000 });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            Console.WriteLine("Opening page to get cookies...");
            await page.GotoAsync(BaseUrl, new PageGotoOptions { Timeout = 60000 });
            await page.WaitForSelectorAsync("body");

            var cookies = await context.CookiesAsync();
            foreach (var c in cookies)
                result[c.Name] = c.Value;

            Console.WriteLine($"Cookies obtained: {string.Join(", ", result.Keys)}");

            await browser.CloseAsync();
            return result;
        }

        public static async Task<List<LalafoProperty>> FetchApiPageAsync(Dictionary<string, string> cookies, int pageNumber = 1)
        {
            string finalUrl = $"{ApiUrl}?expand=url&per-page={ItemsPerPage}&category_id={CategoryId}&sort_by={SortBy}&with_feed_banner=true&page={pageNumber}";
            var request = new HttpRequestMessage(HttpMethod.Get, finalUrl);

            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("device", "pc");
            request.Headers.Add("country-id", "13");
            request.Headers.Add("language", "az_AZ");

            foreach (var kvp in cookies)
                request.Headers.Add("Cookie", $"{kvp.Key}={kvp.Value}");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return new List<LalafoProperty>();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("items", out var items))
            {
                var list = new List<LalafoProperty>();
                foreach (var i in items.EnumerateArray())
                {
                    var itemJsonString = i.GetRawText();
                    var property = JsonSerializer.Deserialize<LalafoProperty>(itemJsonString);
                    if (property != null)
                        list.Add(property);
                }
                return list;
            }
            return new List<LalafoProperty>();
        }

        public static async Task<LalafoProperty> FetchDetailsPageAsync(Dictionary<string, string> cookies, int propertyId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, DetailApiUrl + propertyId + "?expand=url");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            request.Headers.Add("x-request-id", $"react-client_{Guid.NewGuid()}");
            request.Headers.Add("device", "pc");
            request.Headers.Add("experiment", "novalue");
            request.Headers.Add("user-hash", cookies.GetValueOrDefault("event_user_hash", ""));
            request.Headers.Add("country-id", "13");
            request.Headers.Add("language", "az_AZ");
            foreach (var kv in cookies)
                request.Headers.Add("Cookie", $"{kv.Key}={kv.Value}");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new Exception("Response code returned negative");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LalafoProperty>(json)!;
        }

        public static async Task DownloadImagesAsync(JsonElement itemDetail, string baseDir = "./images")
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var title = itemDetail.GetProperty("title").GetString() ?? "untitled";
            var id = itemDetail.GetProperty("id").GetInt32();
            var safeTitle = SanitizeTitle(title);
            var folderName = $"{timestamp}-{safeTitle}-{id}";
            var folderPath = Path.Combine(baseDir, folderName);

            Directory.CreateDirectory(folderPath);
            var images = itemDetail.GetProperty("images").EnumerateArray();

            foreach (var (img, index) in images.Select((v, i) => (v, i + 1)))
            {
                var url = img.GetProperty("original_url").GetString();
                var ext = Path.GetExtension(url);
                var filePath = Path.Combine(folderPath, $"image_{index}{ext}");

                try
                {
                    using var client = new HttpClient();
                    var bytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(filePath, bytes);
                    Console.WriteLine($"Downloaded: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Download failed for {url}: {ex.Message}");
                }
            }
        }

        public static string SanitizeTitle(string title, int maxLength = 100)
        {
            string cleaned = Regex.Replace(title, @"[<>:""/\\|?*]", "");
            cleaned = Regex.Replace(cleaned, @"\s+", " ");
            cleaned = cleaned.Trim(' ', '.');
            if (cleaned.Length > maxLength)
                cleaned = cleaned.Substring(0, maxLength);
            return cleaned;
        }
    }
}
