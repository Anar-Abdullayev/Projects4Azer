using HtmlAgilityPack;
using Microsoft.Playwright;
using UniversalDataCatcher.Server.Bots.YeniEmlak.Models;
using UniversalDataCatcher.Server.Bots.YeniEmlak.StaticConstants;

namespace UniversalDataCatcher.Server.Bots.YeniEmlak.Helpers
{
    public static class YeniEmlakHelper
    {
        private static HttpClient _httpClient = new HttpClient();
        private static IPlaywright playwright;
        private static IPage page;
        public static async Task InitializeChromiumAsync()
        {
            playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true, Timeout = 30000 });
            var context = await browser.NewContextAsync();
            page = await context.NewPageAsync();
        }
        public static async Task<string> GetPageAsync(string url)
        {
            await page.GotoAsync(url);
            await page.WaitForSelectorAsync("body");
            var responseString = await page.ContentAsync();
            return responseString;
        }

        public static async Task<string> GetDetailPageAsync(string url)
        {
            await page.GotoAsync(url);
            await page.WaitForSelectorAsync("price");
            var responseString = await page.ContentAsync();
            return responseString;
        }

        public static async Task<HtmlDocument> GetDocument(string url)
        {
            var htmlString = await GetPageAsync(url);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlString);
            return document;
        }

        public static HtmlNodeCollection? GetAdvItemNodes(HtmlDocument doc)
        {
            var itemNodes = doc.DocumentNode.SelectNodes("//table[contains(@class, 'list')]");
            return itemNodes;
        }

        public static Tuple<string,string,string> GetMainInfosFromAdvItemNode(HtmlNode node)
        {
            var titleNodes = node.SelectNodes(".//titem");
            var tarix = titleNodes[1].SelectSingleNode("./g/b").InnerText;
            var id = titleNodes[2].SelectSingleNode("./g/b").InnerText;
            var advLink = node.SelectSingleNode(".//a[contains(@class, 'detail')]").GetAttributeValue("href", "NotFound");
            return new Tuple<string, string, string>(id, tarix, YeniEmlakConstants.HostUrl+advLink);
        }

        public static YeniEmlakProperty GetPropertyFromDocument(HtmlDocument doc)
        {
            var property = new YeniEmlakProperty();
            
            property.PostType = DocumentHelper.GetPostType(doc);
            property.Price = DocumentHelper.GetPrice(doc);
            property.CreatedAt = DocumentHelper.GetCreateDate(doc);
            property.Description = DocumentHelper.GetDescription(doc);
            property.Address = DocumentHelper.GetAddress(doc);
            property.Rooms = DocumentHelper.GetRoomCount(doc);
            property.Area = DocumentHelper.GetArea(doc);
            property.Floor = DocumentHelper.GetFloor(doc);
            property.LandArea = DocumentHelper.GetLandArea(doc);
            property.Document = DocumentHelper.GetDocument(doc);
            property.BinaType = DocumentHelper.GetBinaType(doc);
            property.Category = DocumentHelper.GetCategory(doc);
            property.PosterName = DocumentHelper.GetPosterName(doc);
            property.PosterType = DocumentHelper.GetPosterType(doc);
            property.PosterPhone = DocumentHelper.GetPosterPhone(doc);
            property.Renovation = DocumentHelper.GetRenovation(doc);

            return property;
        }
    }
}
