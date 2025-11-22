using HtmlAgilityPack;
using System.Threading.Tasks;
using UniversalDataCatcher.Server.Bots.Emlak.Models;

namespace UniversalDataCatcher.Server.Bots.Emlak.Helpers
{
    public static class EmlakHelper
    {
        private static HttpClient client = new HttpClient();

        public static async Task<string> GetPageAsync(string url)
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<HtmlDocument> GetPageDocumentAsync(string url)
        {
            var response = await client.GetAsync(url);
            var responseHtml = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(responseHtml);
            return document;
        }

        public static async Task<HtmlNodeCollection?> GetAdvItemNodes(string url)
        {
            var document = await GetPageDocumentAsync(url);
            var nodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'ticket') and contains(@class, 'clearfix')]");
            return nodes;
        }

        public static Tuple<string, string> GetInitialInfosFromNode(HtmlNode node)
        {
            var advLinkNode = node.SelectSingleNode("./div[@class='img']/a");
            var href = advLinkNode.GetAttributeValue("href", "NotFound");
            var id = href.Split("-")[0].Replace("/", "");
            return new Tuple<string, string>(id, href);
        }

        public static EmlakProperty GetProperty(HtmlDocument doc)
        {
            EmlakDocumentHelper.InitializeDocument(doc);

            EmlakProperty property = new EmlakProperty();

            property.MainTitle = EmlakDocumentHelper.GetMainTitle();
            property.Price = EmlakDocumentHelper.GetPrice();
            property.CreatedAt = EmlakDocumentHelper.GetDate();
            property.Description = EmlakDocumentHelper.GetDescription();
            property.Category = EmlakDocumentHelper.GetTechnicalCharacteristic("Əmlakın növü");
            property.Rooms = EmlakDocumentHelper.GetTechnicalCharacteristic("Otaqların sayı");
            property.Floor = EmlakDocumentHelper.GetTechnicalCharacteristic("Mərtəbə sayı");
            property.ApartmentFloor = EmlakDocumentHelper.GetTechnicalCharacteristic("Yerləşdiyi mərtəbə");
            property.Renovation = EmlakDocumentHelper.GetTechnicalCharacteristic("Təmiri");
            property.Document = EmlakDocumentHelper.GetTechnicalCharacteristic("Sənədin tipi");
            property.PosterName = EmlakDocumentHelper.GetPosterName();
            property.PosterPhone = EmlakDocumentHelper.GetPosterPhone();
            property.PosterType = EmlakDocumentHelper.GetPosterType();
            property.Address = EmlakDocumentHelper.GetAddress();

            string? area = EmlakDocumentHelper.GetTechnicalCharacteristic("Sahə");
            if (area is not null)
            {
                if (area.Contains("m2"))
                    property.Area = area.Replace(" m2","");
                else if (area.Contains("sot"))
                    property.LandArea = area.Replace(" sot","");
            }


            return property;
        }
        public static bool IsValidDetailPage(HtmlDocument doc)
        {
            EmlakDocumentHelper.InitializeDocument(doc);
            var maintitle = EmlakDocumentHelper.GetMainTitle();
            if (string.IsNullOrEmpty(maintitle)) 
                return false;
            if (maintitle == "TitleNodeNotFound")
                return false;
            return true;
        }
    }
}
