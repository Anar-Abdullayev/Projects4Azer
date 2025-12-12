using HtmlAgilityPack;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.RegularExpressions;
using UniversalDataCatcher.Server.Bots.Bina.Models;

namespace UniversalDataCatcher.Server.Bots.Bina.Helpers
{
    public class BinaAzContentHelper
    {
        private static readonly string baseUrl = Constants.Constants.BaseUrl;
        public async Task<string?> GetPage(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                try
                {
                    string html = await client.GetStringAsync(baseUrl + url);
                    return html;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }
            return null;
        }

        public BinaAzPropertyDetails? GetItemDetails(string htmlString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlString);
            var scriptNode = doc.DocumentNode.SelectNodes("//script")?.FirstOrDefault(n => n.InnerText.Contains("currentItemData"));
            if (scriptNode is null)
                return null;
            string script = scriptNode.InnerText;
            string key = "\"currentItemData\"";

            int startIndex = script.IndexOf(key);
            if (startIndex == -1)
            {
                Console.WriteLine("currentItemData not found");
                return null;
            }

            int braceStart = script.IndexOf('{', startIndex);
            if (braceStart == -1)
            {
                Console.WriteLine("Opening brace not found");
                return null;
            }

            int braceCount = 0;
            int i = braceStart;
            for (; i < script.Length; i++)
            {
                if (script[i] == '{') braceCount++;
                if (script[i] == '}') braceCount--;

                if (braceCount == 0)
                {
                    i++;
                    break;
                }
            }
            string jsonText = script.Substring(braceStart, i - braceStart);
            var detailedProperty = JsonSerializer.Deserialize<BinaAzPropertyDetails>(jsonText);
            return detailedProperty;
        }

        public List<Tuple<string, string, string>>? GetPropertiesFromContent(HtmlDocument doc, List<string> formattedDates, ref bool continueSearch)
        {
            int wrongDateCount = 0;
            var productsDiv = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'js-endless-container') and contains(@class,'products')]");
            var divNodes = productsDiv.SelectNodes(".//div[contains(@class, 'products-i')]");

            List<Tuple<string, string, string>> propertyNodes = [];
            if (divNodes != null)
            {
                foreach (var li in divNodes)
                {
                    var aTag = li.SelectSingleNode(".//a");
                    string href = aTag?.GetAttributeValue("href", "") ?? "";
                    string id = "";
                    if (!string.IsNullOrEmpty(href))
                    {
                        id = href.Substring(href.LastIndexOf('/') + 1);
                    }

                    string date = aTag?.SelectSingleNode(".//div[contains(@class,'products-created')]")?.InnerText.Trim() ?? "";


                    if (!formattedDates.Any(fd => date.Contains(fd)))
                    {
                        wrongDateCount++;
                        if (wrongDateCount == divNodes.Count)
                            continueSearch = false;
                        continue;
                    }
                    Tuple<string, string, string> propertyTuple = new(id, href, date);
                    propertyNodes.Add(propertyTuple);
                }
                return propertyNodes;
            }
            return null;
        }

        public async Task<BinaAzProperty> GetPropertyFromRawHTML(string rawHtml, BinaAzProperty property)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(rawHtml);

            property.MainTitle = DocumentHelper.GetMainTitle(doc);
            DocumentHelper.FillProperties(property, doc);
            property.Description = DocumentHelper.GetDescription(doc);
            property.Owner = DocumentHelper.GetOwner(doc);
            property.PhoneNumbers = await DocumentHelper.GetPhoneNumbersAsync(property.Id.ToString()) ?? "";
            property.OwnerType = DocumentHelper.GetOwnerType(doc);
            property.Post_Type = DocumentHelper.GetPostType(doc);
            property.CreatedAt = DocumentHelper.GetCreationTime(doc);
            property.ImageUrls = DocumentHelper.GetImageUrls(doc);

            return property;
        }
    }
}
