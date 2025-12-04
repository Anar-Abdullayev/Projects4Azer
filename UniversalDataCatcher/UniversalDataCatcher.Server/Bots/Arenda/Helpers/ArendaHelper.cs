using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using UniversalDataCatcher.Server.Services.Arenda.Model;

namespace UniversalDataCatcher.Server.Services.Arenda.Helpers
{
    public static class ArendaHelper
    {
        private static readonly string url = "https://arenda.az/filtirli-axtaris/XXPAGEXX/?home_search=1&lang=1&site=1&home_s=1&price_min=&price_max=&sahe_min=&sahe_max=&mertebe_min=&mertebe_max=&y_mertebe_min=&y_mertebe_max=&axtar=&order=2";

        public static async Task<string?> GetPage(int page)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                try
                {
                    string html = await client.GetStringAsync(url.Replace("XXPAGEXX", page.ToString()));
                    return html;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }
            return null;
        }

        public static async Task<string?> GetPropertyDetailPage(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                try
                {
                    string html = await client.GetStringAsync(url);
                    return html;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }
            return null;
        }

        public static List<Tuple<string, string, string>>? GetPropertiesFromContent(string htmlContent, List<string> formattedDates, ref bool continueSearch)
        {
            int wrongDateCount = 0;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            var liNodes = doc.DocumentNode.SelectNodes("//ul[contains(@class,'a_netice') and contains(@class,'elan_list')]/li");

            List<Tuple<string, string, string>> propertyNodes = [];
            if (liNodes != null)
            {
                foreach (var li in liNodes)
                {
                    string liId = li.GetAttributeValue("id", "");
                    var aTag = li.SelectSingleNode(".//a");
                    string href = aTag?.GetAttributeValue("href", "") ?? "";
                    string date = aTag?.SelectSingleNode(".//span[contains(@class,'elan_box_date')]")?.InnerText.Trim() ?? "";


                    if (!formattedDates.Any(fd => date.Contains(fd)))
                    {
                        wrongDateCount++;
                        if (wrongDateCount == liNodes.Count)
                            continueSearch = false;
                        continue;
                    }
                    Tuple<string, string, string> propertyTuple = new(liId, href, date);
                    propertyNodes.Add(propertyTuple);
                }
                return propertyNodes;
            }
            return null;
        }

        public static ArendaProperty GetPropertyFromRawHTML(string html)
        {
            ArendaProperty property = new ArendaProperty();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var titleSideNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'elan_title_bg')]//div[contains(@class,'elan_title_left')]");
            var leftSideNode = doc.DocumentNode.SelectSingleNode("//section[contains(@class,'elan_desc_sides') and contains(@class,'elan_desc_left_side')]");
            var rightSideNode = doc.DocumentNode.SelectSingleNode("//section[contains(@class,'elan_desc_sides') and contains(@class,'elan_desc_right_side') and contains (@class,'elan_in_right')]");
            var imageGalleryNode = doc.DocumentNode.SelectSingleNode("//section[@id='elan_gallery']");


            // Left side
            var mainTitle = DocumentHelper.GetMainTitle(titleSideNode);
            var secondaryTitle = DocumentHelper.GetSecondaryTitle(leftSideNode);
            var propertyMainInfos = DocumentHelper.GetMainPropertyInfos(leftSideNode);
            var roomInfo = propertyMainInfos?.FirstOrDefault(info => info.ToLower().Contains("otaq"));
            var sizeInfo = propertyMainInfos?.FirstOrDefault(info => info.ToLower().Contains(" m2"));
            int? roomCount = roomInfo == null ? null : int.Parse(roomInfo.Replace(" otaq", ""));
            float? propertySize = sizeInfo == null ? null : float.Parse(sizeInfo.Replace(" m2", ""), CultureInfo.InvariantCulture);
            var description = DocumentHelper.GetDescription(leftSideNode);
            var propertyFeatures = DocumentHelper.GetPropertyFeatures(leftSideNode);
            var address = DocumentHelper.GetAddress(leftSideNode);
            var mainAddress = DocumentHelper.GetMainAddress(titleSideNode);
            var imageUrls = DocumentHelper.GetImageUrls(imageGalleryNode);

            // Right side
            var price = DocumentHelper.GetPrice(rightSideNode);
            var owner = DocumentHelper.GetOwner(rightSideNode);
            var contactNumbers = DocumentHelper.GetContactNumbers(rightSideNode);

            property.MainTitle = mainTitle;
            property.SecondaryTitle = secondaryTitle;
            property.PropertyMainInfos = propertyMainInfos;
            property.RoomCount = roomCount;
            property.PropertySize = propertySize;
            property.Description = description;
            property.PropertyFeatures = propertyFeatures;
            property.Address = mainAddress+" "+address;
            property.Poster_Type = owner.Contains("Əmlak sahibi") ? "mülkiyyətçi" : "vasitəçi";
            property.Price = price;
            property.Owner = owner.Replace(" (Əmlak sahibi)", "").Replace(" (Vasitəçi)","");
            property.ContactNumbers = contactNumbers is not null && contactNumbers.Count > 0 ? String.Join(", ", contactNumbers.ToArray()) : "Yoxdur";
            property.ContactNumbers = property.ContactNumbers.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
            property.ImageUrls = imageUrls;

            var floorString = property.PropertyMainInfos?.FirstOrDefault(x => x.ToLower().Contains("mərtəbə"));
            if (!string.IsNullOrEmpty(floorString))
                property.Floor = floorString.Split('/')[0].Replace(" ","").Replace("mərtəbəli", "");

            var torpaqString = property.PropertyMainInfos?.FirstOrDefault(y => y.ToLower().Contains("sot"));
            if (torpaqString != null)
                property.TorpaqArea = torpaqString.Replace(" sot", "");

            var documentString = property.PropertyMainInfos?.FirstOrDefault(y => y.ToLower().Contains("çıxarış"));
            if (documentString != null)
                property.Document = "var";

            var repairString = property.PropertyFeatures?.FirstOrDefault(y => y.ToLower().Contains("təmirli"));
            if (repairString != null)
                property.Repair = "var";


            property.Post_Type = property.MainTitle.StartsWith("Satılır") ? "Satış" : property.MainTitle.StartsWith("Kirayə") ? "Kirayə" : "Bilinmir";

            return property;
        }

    }
}
