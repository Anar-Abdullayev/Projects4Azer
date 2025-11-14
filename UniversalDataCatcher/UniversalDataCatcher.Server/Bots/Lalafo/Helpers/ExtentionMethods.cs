using UniversalDataCatcher.Server.Bots.Lalafo.Models;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Helpers
{
    public static class ExtentionMethods
    {
        public static void PrintDetails(this LalafoProperty property)
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Id: " + property.Id);
            Console.WriteLine("Url: " + LalafoHelper.WebsiteUrl + property.Url);
            Console.WriteLine("City: "+ property.City);
            Console.WriteLine("Price: "+ property.Price);
            Console.WriteLine("Created at: "+ DateTimeOffset.FromUnixTimeSeconds(property.CreatedTime).ToString());
            Console.WriteLine("Title: "+ property.Title);
            Console.WriteLine("Description: "+ property.Description);
            Console.WriteLine("Parameters:");
            foreach (var kvp in property.Parameters)
            {
                Console.WriteLine($"{kvp.Name}: {kvp.Value}");
            }
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("");
        }
    }
}
