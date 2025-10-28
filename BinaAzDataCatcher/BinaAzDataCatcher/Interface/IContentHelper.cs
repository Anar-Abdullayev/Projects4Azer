using BinaAzDataCatcher.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaAzDataCatcher.Interface
{
    public interface IContentHelper<T> where T : class
    {
        Task<string?> GetPage(string url);
        List<Tuple<string, string, string>>? GetPropertiesFromContent(HtmlDocument doc, List<string> formattedDates, ref bool continueSearch);
        Task<T> GetPropertyFromRawHTML(string rawHtml, BinaAzProperty property);
    }
}
