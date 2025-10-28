using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCatcherBot
{
    public static class DocumentHelper
    {
        public static string GetMainTitle(HtmlNode node)
        {
            var titleNode = node.SelectSingleNode(".//h2[contains(@class,'elan_in_title_link') and contains(@class,'elan_main_title')]");
            if (titleNode != null)
                return HtmlEntity.DeEntitize(titleNode.InnerText.Trim());
            return "";
        }

        public static string GetSecondaryTitle(HtmlNode node)
        {
            var titleNode = node.SelectSingleNode(".//h3[contains(@class,'elan_in_title_link')]");
            if (titleNode != null)
                return HtmlEntity.DeEntitize(titleNode.InnerText.Trim());
            return "";
        }

        public static List<string>? GetMainPropertyInfos(HtmlNode node)
        {
            var infos = new List<string>();
            var ulNode = node.SelectSingleNode("//ul[@class='full elan_property_list']");
            var links = ulNode.SelectNodes(".//a");
            if (links == null)
                return null;
            return links.Select(a => HtmlEntity.DeEntitize(a.InnerText.Trim())).ToList();
        }

        public static string GetDescription(HtmlNode node)
        {
            var descNode = node.SelectSingleNode(".//div[contains(@class,'full') and contains(@class,'elan_info_txt')]/p");
            if (descNode != null)
                return HtmlEntity.DeEntitize(descNode.InnerText.Trim());
            return "";
        }

        public static List<string>? GetPropertyFeatures(HtmlNode node)
        {
            var features = node
            .SelectNodes("//ul[@class='full property_lists']/li")
            ?.Select(li => HtmlEntity.DeEntitize(li.InnerText.Trim()))
            .ToList();

            return features;
        }
        
        public static string GetAddress(HtmlNode node)
        {
            var addressNode = node.SelectSingleNode(".//span[contains(@class,'elan_unvan_txt')]");
            if (addressNode != null)
                return HtmlEntity.DeEntitize(addressNode.InnerText.Trim());
            return "";
        }

        public static float GetPrice(HtmlNode node)
        {
            var priceNode = node.SelectSingleNode(".//div[contains(@class,'elan_new_price_box')]/p");
            if (priceNode != null)
            {
                string priceText = priceNode.InnerText.Replace("M", "").Replace(" ", "");
                if (float.TryParse(priceText, out float price))
                    return price;
            }
            return 0;
        }

        public static string GetOwner(HtmlNode node)
        {
            var ownerNode = node.SelectSingleNode(".//div[contains(@class,'new_elan_user_info')]/p");
            if (ownerNode != null)
                return HtmlEntity.DeEntitize(ownerNode.InnerText.Trim());
            return "";
        }  

        public static List<string>? GetContactNumbers(HtmlNode node)
        {
            var numbers = node
            .SelectNodes("//p[@class='elan_in_tel_box']/a")
            ?.Select(a => a.InnerText.Trim())
            .ToList();
            return numbers;
        }
    }
}
