using HtmlAgilityPack;
using System.Xml.Linq;

namespace UniversalDataCatcher.Server.Bots.YeniEmlak.Helpers
{
    public static class DocumentHelper
    {
        public static string? GetPostType(HtmlDocument doc)
        {
            var typeNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'title')]/tip");
            var typeString = typeNode?.InnerText;
            return typeString;
        }

        public static string? GetPrice(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'title')]/price");
            var priceString = titleNode?.InnerText;
            return priceString;
        }

        public static DateTime GetCreateDate(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'title')]");
            if (titleNode is null)
                return DateTime.MinValue;
            var dateString = titleNode.SelectNodes(".//titem")[1].SelectSingleNode("./g/b").InnerText;
            var date = DateTime.ParseExact(dateString, "dd.MM.yyyy", null);
            return date;
        }

        public static string? GetDescription(HtmlDocument doc)
        {
            var boxNode = doc.DocumentNode.SelectNodes("//div[contains(@class, 'box')]")?[0];
            if (boxNode is null)
                return null;
            var descriptionNode = boxNode.SelectNodes(".//div[contains(@class, 'text')]")[0];
            var descriptionString = descriptionNode.InnerText;
            return descriptionString;
        }

        public static string? GetAddress(HtmlDocument doc)
        {
            var addressParams = doc.DocumentNode.SelectNodes(".//h1[normalize-space(text())='Ünvan']/following-sibling::div[@class='params']");
            var addressManualText = doc.DocumentNode.SelectSingleNode(".//h1[normalize-space(text())='Ünvan']/following-sibling::div[@class='text']");
            string address = "";
            List<string> addresses = new List<string>();
            if (addressParams is not null)
                foreach (var addressParam in addressParams)
                {
                    addresses.Add(addressParam.SelectSingleNode("./b").InnerText);
                }
            if (addressManualText is not null)
                addresses.Add(addressManualText.InnerText);
            var joinedString = String.Join(',', [.. addresses]);
            return joinedString;
        }

        public static string? GetRoomCount(HtmlDocument doc)
        {
            var roomNode = doc.DocumentNode.SelectSingleNode("//div[@class='params' and contains(normalize-space(.), 'otaq')]");

            if (roomNode is null)
                return null;

            var roomCount = roomNode.InnerText.Replace(" otaq", "");
            return roomCount;
        }

        public static string? GetArea(HtmlDocument doc)
        {
            var areaNode = doc.DocumentNode.SelectSingleNode("//div[@class='params' and contains(normalize-space(.), 'm2')]");

            if (areaNode is null)
                return null;

            var areaCount = areaNode.InnerText.Replace(" m2", "");
            return areaCount;
        }

        public static string? GetFloor(HtmlDocument doc)
        {
            var floorNode = doc.DocumentNode.SelectSingleNode("//div[@class='params' and contains(normalize-space(.), 'Mərtəbə')]");

            if (floorNode is null)
                return null;

            var floor = floorNode.InnerText.Replace(" Mərtəbə", "");
            if (floor.Contains("/"))
            {
                var totalFloors = floor.Split(" / ")[0];
                var apartmentFloor = floor.Split(" / ")[1];
                return apartmentFloor;
            }
            return floor.Replace("li", "");
        }

        public static string? GetLandArea(HtmlDocument doc)
        {
            var areaNode = doc.DocumentNode.SelectSingleNode("//div[@class='params' and substring(normalize-space(.), string-length(normalize-space(.)) - 2) = 'sot']");

            if (areaNode is null)
                return null;

            var areaCount = areaNode.InnerText.Replace(" sot", "");
            return areaCount;
        }

        public static string? GetDocument(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode.SelectSingleNode("//div[@class='params' and contains(normalize-space(.), 'Kupça')]");
            if (documentNode is null)
                return null;
            if (documentNode.InnerText == "Kupça")
                return "var";
            if (documentNode.InnerText == "Kupçasız")
                return "yox";
            return documentNode.InnerText;
        }

        public static string? GetBinaType(HtmlDocument doc)
        {
            var emlakNode = doc.DocumentNode.SelectSingleNode("//emlak");
            var emlak = emlakNode?.InnerText.Trim();
            return emlak;
        }

        public static string? GetCategory(HtmlDocument doc)
        {
            var subTypeNode = doc.DocumentNode.SelectSingleNode(".//emlak/following-sibling::text()[1]");
            var subType = subTypeNode?.InnerText.Replace("/ ", "").Trim();
            return subType;
        }

        public static string? GetPosterName(HtmlDocument doc)
        {
            var posterNode = doc.DocumentNode.SelectSingleNode("//div[@class='ad']");
            if (posterNode is null) return null;

            return posterNode.InnerText.Replace("\n", "").Trim();
        }

        public static string? GetPosterType(HtmlDocument doc)
        {
            var posterTypeNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'elvrn')]");
            if (posterTypeNode is null) return null;

            if (posterTypeNode.InnerText.Contains("Vasitəçi"))
                return "vasitəçi";
            if (posterTypeNode.InnerText.Contains("Əmlak sahibi"))
                return "mülkiyyətçi";
            return posterTypeNode.InnerText.Trim();
        }

        public static string? GetPosterPhone(HtmlDocument doc)
        {
            var phoneImg = doc.DocumentNode.SelectNodes(".//div[@class='tel']//img");
            var src = phoneImg?[0].GetAttributeValue("src", "");

            string? phone = null;

            if (!string.IsNullOrWhiteSpace(src))
            {
                phone = new string(src.Where(char.IsDigit).ToArray());
            }
            return phone;
        }

        public static string? GetRenovation(HtmlDocument doc)
        {
            var renovationNode = doc.DocumentNode.SelectSingleNode("//div[@class='check' and normalize-space(text())='Təmirli']");
            if (renovationNode is null) return null;
            return "təmirli";
        }

        public static string? GetImageUrls(HtmlDocument doc)
        {
            var imageNodes = doc.DocumentNode.SelectNodes("//div[@class='imgbox']/div/a");
            if (imageNodes is not null && imageNodes.Count > 0)
            {
                var imageUrls = imageNodes.Select(a => a.GetAttributeValue("href", ""));
                return string.Join(", ", imageUrls);
            }
            return null;
        }
    }
}
