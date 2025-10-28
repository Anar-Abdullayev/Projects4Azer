using BinaAzDataCatcher.Interface;
using BinaAzDataCatcher.Models;
using HtmlAgilityPack;

namespace BinaAzDataCatcher.Helpers
{
    public class BinaAzContentHelper : IContentHelper<BinaAzProperty>
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

            return property;
        }

    }
}
