using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace UniversalDataCatcher.Server.Bots.EvTen.Helpers
{
    public static class EvTenHelper
    {
        public static async Task<string> GetPageAsync(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        public static string ExtractNextFData(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var blocks = doc.DocumentNode
                .SelectNodes("//script[contains(text(), '__next_f.push')]")
                ?.Select(x => x.InnerText)
                .ToList() ?? new List<string>();

            var dataChunks = new List<string>();

            foreach (var block in blocks)
            {
                int start = block.IndexOf("\"");
                int end = block.LastIndexOf("\"");
                if (start >= 0 && end > start)
                    dataChunks.Add(block.Substring(start + 1, end - start - 1));
            }

            return string.Concat(dataChunks);
        }

        public static Dictionary<string, string> ParseKeyValueMap(string merged)
        {
            var dict = new Dictionary<string, string>();

            var matches = Regex.Matches(
                merged,
                @"([0-9a-f]{1,3}):(\{.*?\}|\[.*?\])",
                RegexOptions.Singleline);

            foreach (Match m in matches)
            {
                string key = m.Groups[1].Value;
                string json = m.Groups[2].Value;
                dict[key] = json;
            }

            return dict;
        }
        public static Dictionary<string, string> ParseKeyValueMap2(string merged)
        {
            var dict = new Dictionary<string, string>();
            int i = 0;

            while (i < merged.Length)
            {
                // 1. Read key (example: 39:)
                int colonIndex = merged.IndexOf(':', i);
                if (colonIndex == -1) break;

                string key = merged.Substring(i, colonIndex - i);
                key = key.Replace("\\n", "");

                // Move cursor after colon
                int start = colonIndex + 1;

                // 2. Detect JSON start
                if (merged[start] != '{' && merged[start] != '[')
                {
                    i = start + 1;
                    continue;
                }

                char openChar = merged[start];
                char closeChar = openChar == '{' ? '}' : ']';

                int depth = 0;
                bool insideString = false;

                int j = start;

                // 3. Walk until full JSON is captured
                while (j < merged.Length)
                {
                    char c = merged[j];

                    if (c == '"' && merged[j - 1] != '\\')
                    {
                        insideString = !insideString;
                    }

                    if (!insideString)
                    {
                        if (c == openChar) depth++;
                        else if (c == closeChar) depth--;

                        if (depth == 0)
                        {
                            j++; // include the closing bracket
                            break;
                        }
                    }

                    j++;
                }

                string json = merged.Substring(start, j - start);
                dict[key] = json;

                i = j; // move to next entry
            }

            return dict;
        }

        public static string ResolveReferences(string json, Dictionary<string, string> dict)
        {
            bool changed = true;

            while (changed)
            {
                changed = false;

                var matches = Regex.Matches(json, @"\$([0-9a-f]{1,3})");

                foreach (Match m in matches)
                {
                    string refKey = m.Groups[1].Value;

                    if (dict.TryGetValue(refKey, out string replacement))
                    {
                        json = json.Replace($"\"${refKey}\"", replacement);
                        json = json.Replace($"${refKey}", replacement);
                        changed = true;
                    }
                }
            }

            return json;
        }

        public static List<string> ExtractPostingKeys(Dictionary<string, string> dict)
        {
            if (!dict.TryGetValue("3b", out var postingsArray))
                return new List<string>();

            var matches = Regex.Matches(postingsArray, @"\$([0-9a-f]{1,3})");

            return matches
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToList();
        }

        public static List<string> BuildFullObjects(
    Dictionary<string, string> dict)
        {
            var results = new List<string>();
            var postingKeys = ExtractPostingKeys(dict);

            foreach (string key in postingKeys)
            {
                if (!dict.TryGetValue(key, out var itemJson))
                    continue;

                string resolved = ResolveReferences(itemJson, dict);
                results.Add(resolved);
            }

            return results;
        }

        public static string ExtractPostingKey(string mergedString)
        {
            var firstPostingMatch = Regex.Match(
    mergedString,
    @"\\\""posting\\\"":\\\""(\$\w+)\\\""",
    RegexOptions.Singleline
);

            if (!firstPostingMatch.Success)
                throw new Exception("No posting placeholder found.");

            string postingPlaceholder = firstPostingMatch.Groups[1].Value;
            return postingPlaceholder;
        }

        public static string ReplacePlaceholders(string json, Dictionary<string, string> dict)
        {
            var pattern = @"\$(\w+)";
            string result = json;
            bool replaced;

            do
            {
                replaced = false;
                result = Regex.Replace(result, pattern, match =>
                {
                    var key = match.Groups[1].Value;
                    if (key == "$3b")
                        return key;
                    if (dict.TryGetValue(key, out var value))
                    {
                        replaced = true;
                        return value;
                    }
                    return match.Value;
                });
            } while (replaced);

            return result;
        }
        public static string FixJsonString(string json)
        {
            json = Regex.Unescape(json);
            json = json.Replace("\"[", "[");
            json = json.Replace("]\"", "]");
            json = json.Replace("\"{", "{");
            json = json.Replace("}\"", "}");
            return json;
        }
    }
}
