using BinaAzDataCatcher.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BinaAzDataCatcher.Helpers
{
    public class BinaAzHelper
    {
        private static readonly CookieContainer cookieContainer = new CookieContainer();
        private static readonly HttpClientHandler handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            UseCookies = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        private static readonly HttpClient client = new HttpClient(handler)
        {
            BaseAddress = new Uri(Constants.Constants.BaseUrl)
        };

        public static async Task StartInitialRun()
        {
            // Step 1: mimic a browser
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            // Step 2: First request to get cookies
            var initialResponse = await client.GetAsync("/items/all");
            Console.WriteLine("Initial request status: " + initialResponse.StatusCode);

            var html = await initialResponse.Content.ReadAsStringAsync();
            Console.WriteLine("HTML length: " + html.Length);

            // Step 3: Print cookies
            foreach (Cookie cookie in cookieContainer.GetCookies(new Uri("https://bina.az")))
            {
                Console.WriteLine($"Cookie: {cookie.Name} = {cookie.Value}");
            }

            // Step 4: Now request GraphQL with cookies automatically attached
            client.DefaultRequestHeaders.Add("X-APOLLO-OPERATION-NAME", "SearchItems");
        }
        public static async Task<BinaAzResponseRoot?> GetData(GraphqlQueryParams queryParams)
        {
            var queryParamString = queryParams.ToQueryString();
            var apiResponse = await client.GetAsync($"/graphql?{queryParamString}");
            var apiBody = await apiResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<BinaAzResponseRoot>(apiBody);
            if (data is not null && data.Data.ItemsConnection != null)
            {
                if (data.Data.ItemsConnection.PageInfo.HasNextPage)
                {
                    queryParams.variables.cursor = data.Data.ItemsConnection.PageInfo.EndCursor;
                }
            }
            return data;
        }
    }
}
