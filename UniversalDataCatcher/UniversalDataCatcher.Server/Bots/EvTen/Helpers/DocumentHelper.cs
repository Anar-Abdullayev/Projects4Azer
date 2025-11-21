using HtmlAgilityPack;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace UniversalDataCatcher.Server.Bots.EvTen.Helpers
{
    public static class DocumentHelper
    {
        public static string? GetMainTitle(string htmlContent)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            var titleNode = doc.DocumentNode.SelectNodes("//h1[contains(@class, 'mui-x0db0x')]");
            if (titleNode != null)
            {
                return titleNode[0].InnerText.Trim();
            }
            return null;
        }

        public static string GetDescriptionFromMergedString(string merged)
        {
            var startingIndex = merged.IndexOf("3b:");
            var endingIndex = merged.IndexOf("3c:");
            var description = merged.Substring(startingIndex, endingIndex-startingIndex);
            description = description.Substring(8);
            return string.IsNullOrEmpty(description) ? "" : description;
        }

        public static bool HasIpotekaInfo(string htmlContent)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            var ipotekaNode = doc.DocumentNode.SelectSingleNode("//span[text()='İpoteka hesabla']");
            return ipotekaNode != null;
        }
    }
}
