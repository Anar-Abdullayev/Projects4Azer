using HtmlAgilityPack;
using System.Text.Json;
using UniversalDataCatcher.Server.Bots.Tap.Models;

namespace UniversalDataCatcher.Server.Bots.Tap.Helpers
{
    public static class DocumentHelper
    {
        private static readonly string phoneUrl = "https://tap.az/ads/XXIDXX/phones";
        public static void FillProperties(TapAzProperty property, HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-properties__column')]/div[contains(@class, 'product-properties__i')]");

            foreach (var node in nodes)
            {
                var label = node.SelectSingleNode(".//label").InnerText;
                var value = node.SelectSingleNode(".//span").InnerText;

                if (label.StartsWith("Şəhər"))
                    property.City = value;
                else if (label.StartsWith("Yerləş"))
                    property.Address = value;
                else if (label.StartsWith("Elanın tipi"))
                    property.AdvType = value;
                else if (label.StartsWith("Əmlakın növü"))
                    property.PropType = value;
                else if (label.StartsWith("Sahə, sot"))
                    property.LandArea = value;
                else if (label.StartsWith("Sahə"))
                    property.Area = value;
                else if (label.StartsWith("Kirayə müddəti"))
                    property.RentLong = value;
                else if (label.StartsWith("Binanın tipi"))
                    property.BuildingType = value;
                else if (label.StartsWith("Otaq sayı"))
                    property.RoomCount = value;
            }
        }

        public static string GetMainTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'product-title')]");
            return titleNode?.InnerText.Trim() ?? "";
        }

        public static string GetDescription(HtmlDocument doc)
        {
            var propertyDescriptionNode = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-description-container')]//p");
            var description = "";
            if (propertyDescriptionNode is not null)
            {
                foreach (var para in propertyDescriptionNode)
                {
                    description += para.InnerText.Trim() + "\n";
                }
            }
            return string.IsNullOrEmpty(description) ? "No Description" : description;
        }

        public static string GetPrice(HtmlDocument doc)
        {
            var propertyPriceNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-price')]");
            return propertyPriceNode.SelectSingleNode("//span[contains(@class, 'price-val')]").InnerText.Trim() + " " + propertyPriceNode.SelectSingleNode("//span[contains(@class, 'price-cur')]").InnerText.Trim();
        }

        public static string GetOwner(HtmlDocument doc)
        {
            var ownerNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-owner__info-name')]");
            if (ownerNode is null)
                ownerNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'product-shop__owner-name')]");
            return ownerNode.InnerText.Trim();
        }

        public static async Task<string> GetOwnerType(HtmlDocument doc)
        {
            var ownerTypeNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'product-shop__owner-ads')]");
            string ownerType = "mülkiyyətçi";
            if (ownerTypeNode is null)
                ownerTypeNode = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'product-owner__info-ads')]");
            if (ownerTypeNode is not null)
            {
                if (ownerTypeNode.HasClass("product-owner__info-ads"))
                {
                    var tapazHelper = new TapazHelper();
                    var ownerAdvsPageString = await tapazHelper.GetPage(ownerTypeNode.GetAttributeValue("href", ""));
                    if (ownerAdvsPageString is not null)
                    {
                        HtmlDocument ownerAdvsDoc = new HtmlDocument();
                        ownerAdvsDoc.LoadHtml(ownerAdvsPageString);
                        var ownerAdvLinks = ownerAdvsDoc.DocumentNode.SelectNodes("//div[contains(@class, 'products-i') and contains(@class, 'rounded')]/a");
                        var houseCount = 0;
                        if (ownerAdvLinks is not null)
                        {
                            foreach (var advLink in ownerAdvLinks)
                            {
                                if (advLink.GetAttributeValue("href", "").Contains("dasinmaz-emlak"))
                                    houseCount++;
                                if (houseCount > 2)
                                {
                                    ownerType = "vasitəçi";
                                    break;
                                }
                            }

                        }
                    }
                }
                else
                    ownerType = "vasitəçi";
            }
            return ownerType;
        }

        public static async Task<string?> GetPhoneNumbersAsync(string advId)
        {
            string url = phoneUrl.Replace("XXIDXX", advId);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                try
                {
                    var response = await client.PostAsync(url, null);
                    response.EnsureSuccessStatusCode();

                    string payload = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(payload);
                    if (!doc.RootElement.TryGetProperty("phones", out JsonElement phonesElement))
                        return null;

                    var phoneNumbers = new List<string>();

                    foreach (var phone in phonesElement.EnumerateArray())
                    {
                        phoneNumbers.Add(phone.GetString() ?? string.Empty);
                    }

                    return string.Join(", ", phoneNumbers);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching phone numbers: {ex.Message}");
                    return null;
                }
            }
        }

        public static string GetNextPageUrl(HtmlDocument doc)
        {
            var nextPageNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'pagination')]/div/a");
            return nextPageNode?.GetAttributeValue("href", "") ?? "";
        }
    }
}
