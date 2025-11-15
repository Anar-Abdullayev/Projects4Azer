using System.Net;
using System.Text.Json;
using UniversalDataCatcher.Server.Bots.Bina.Models;

namespace UniversalDataCatcher.Server.Bots.Bina.Extentions
{
    public static class ExtentionMethods
    {
        public static string ToQueryString(this GraphqlQueryParams query)
        {
            var operationName = "operationName=" + WebUtility.UrlEncode(query.operationName);
            var variables = "variables=" + WebUtility.UrlEncode(JsonSerializer.Serialize(query.variables));
            var extensions = "extensions=" + WebUtility.UrlEncode(JsonSerializer.Serialize(query.extensions));
            return $"{operationName}&{variables}&{extensions}";
        }
    }
}
