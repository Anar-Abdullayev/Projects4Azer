using HtmlAgilityPack;
using System.Xml.Linq;
using UniversalDataCatcher.Server.Bots.Emlak.StaticConstants;

namespace UniversalDataCatcher.Server.Bots.Emlak.Helpers
{
    public static class EmlakDocumentHelper
    {
        private static HtmlDocument doc;
        public static void InitializeDocument(HtmlDocument _doc) => doc = _doc;
        public static string GetMainTitle()
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='title']");
            if (titleNode is null)
                return "TitleNodeNotFound";
            var mainTitle = titleNode.InnerText;
            return mainTitle;
        }
        public static string GetPrice()
        {
            var priceNode = doc.DocumentNode.SelectSingleNode("//div[@class='price']/span[@class='m']");
            if (priceNode is null)
                return "PriceNodeNotFound";
            var price = priceNode.InnerText.Replace(" ", "");
            return price;
        }

        public static DateTime GetDate()
        {
            var dateNode = doc.DocumentNode.SelectSingleNode("//span[@class='date']/strong");
            var date = DateTime.ParseExact(dateNode.InnerText, "dd.MM.yyyy", null);
            return date;
        }

        public static string? GetDescription()
        {
            var descriptionNode = doc.DocumentNode.SelectSingleNode("//div[@class='desc']/p");
            var description = descriptionNode?.InnerText;
            return description;
        }

        public static string? GetTechnicalCharacteristic(string characteristic)
        {
            var techsNode = doc.DocumentNode.SelectNodes("//dl[@class='technical-characteristics']/dd");
            if (techsNode is not null)
                foreach (var tech in techsNode)
                {
                    if (tech.InnerText.Contains(characteristic))
                    {
                        return tech.InnerText.Replace(characteristic, "");
                    }
                }
            return null;
        }

        public static string? GetPosterName()
        {
            var posterNameNode = doc.DocumentNode.SelectSingleNode("//div[@class='silver-box']/p[@class='name-seller']");
            string? posterName = null;
            if (posterNameNode is not null && posterNameNode.ChildNodes is not null)
                posterName = posterNameNode.ChildNodes
                     .Where(n => n.NodeType == HtmlNodeType.Text)
                     .FirstOrDefault()?.InnerText
                     ?.Trim();
            return posterName;
        }

        public static string? GetPosterPhone()
        {
            var posterPhoneNode = doc.DocumentNode.SelectSingleNode("//div[@class='silver-box']/p[@class='phone']");
            var posterPhone = posterPhoneNode?.InnerText;
            return posterPhone;
        }

        public static string? GetPosterType()
        {
            var posterNameNode = doc.DocumentNode.SelectNodes("//div[@class='silver-box']/p[@class='name-seller']/span");
            if (posterNameNode is not null)
                return posterNameNode[0].InnerText;
            return null;
        }

        public static string? GetAddress()
        {
            var addressNode = doc.DocumentNode.SelectSingleNode("//div[@class='map-address']/h4");
            var address = addressNode?.InnerText;
            return address;
        }
        public static string? GetImageUrls()
        {
            var imgNodes = doc.DocumentNode.SelectNodes("//div[@class = 'fotorama']//img");

            if (imgNodes != null)
            {
                var imageUrls = imgNodes.Select(node => EmlakConstants.HostUrl + node.GetAttributeValue("src", ""));
                return string.Join(", ", imageUrls);
            }
            return null;
        }
    }
}
