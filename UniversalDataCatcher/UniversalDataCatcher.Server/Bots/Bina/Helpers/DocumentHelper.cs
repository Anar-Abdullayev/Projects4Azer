using HtmlAgilityPack;
using System.Globalization;
using System.Text.Json;
using UniversalDataCatcher.Server.Bots.Bina.Models;

namespace UniversalDataCatcher.Server.Bots.Bina.Helpers
{
    public class DocumentHelper
    {
        private static readonly string phoneUrl = "https://bina.az/items/XXIDXX/phones?source_link=https%3A%2F%2Fbina.az%2Fitems%2FXXIDXX&trigger_button=main";
        public static void FillProperties(BinaAzProperty property, HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-properties__column')]/div[contains(@class, 'product-properties__i')]");

            foreach (var node in nodes)
            {
                var label = node.SelectSingleNode(".//label").InnerText;
                var value = node.SelectSingleNode(".//span").InnerText;

                if (label.StartsWith("Kateqoriya"))
                    property.Category = value;
                else if (label.StartsWith("Kirayə müddəti"))
                    property.RentLong = value;
                else if (label.StartsWith("Təmir"))
                    property.Repair = value;
                else if (label.StartsWith("Çıxarış"))
                    property.Cixaris = value;
                else if (label.StartsWith("Binanın növü"))
                    property.BuildingType = value;
                else if (label.StartsWith("Torpaq sah"))
                    property.LandArea = value;
                else if (label.StartsWith("İpoteka"))
                    property.Ipoteka = value;
            }
        }

        public static string GetMainTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'product-title')]");
            return titleNode?.InnerText.Trim() ?? "";
        }

        public static string GetDescription(HtmlDocument doc)
        {
            var propertyDescriptionNode = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-description__content')]//p");
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
            if (ownerNode is null)
                ownerNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-owner__residence-info-name')]");
            return ownerNode.InnerText.Trim();
        }

        public static string GetOwnerType(HtmlDocument doc)
        {
            var ownerTypeNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-owner__info-region')]");
            var ownerType = "";
            if (ownerTypeNode is not null)
                ownerType = ownerTypeNode.InnerText.Trim();
            return ownerType;
        }

        public static string GetPostType(HtmlDocument doc)
        {
            var postTypeNode = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'product-breadcrumbs__i-link')]");
            var postType = "";
            if (postTypeNode is not null)
                postType = postTypeNode.InnerText.Trim();
            return postType;
        }

        public static string GetCreationTime(HtmlDocument doc)
        {
            var creationTimeNode = doc.DocumentNode.SelectNodes("//span[contains(@class, 'product-statistics__i-text')]");
            var createdAtString = "";
            if (creationTimeNode is not null)
                createdAtString = creationTimeNode[1].InnerText.Trim().Replace("Yeniləndi: ","");
            DateTime createdAt = DateTime.ParseExact(createdAtString, "dd.MM.yyyy, HH:mm",
                                                CultureInfo.InvariantCulture);
            return createdAt.ToString();
        }

        public static async Task<string?> GetPhoneNumbersAsync(string advId)
        {
            string url = phoneUrl.Replace("XXIDXX", advId);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                try
                {
                    var response = await client.GetAsync(url);
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
    }
}
