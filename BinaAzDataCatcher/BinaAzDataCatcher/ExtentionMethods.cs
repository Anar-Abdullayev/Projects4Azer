using BinaAzDataCatcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BinaAzDataCatcher
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
