using BinaAzDataCatcher.Database;
using BinaAzDataCatcher.Helpers;
using BinaAzDataCatcher.Models;
using System.Text;

namespace BinaAzDataCatcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var helper = new BinaAzContentHelper();
            var database = new MSSqlDatabaseService();
            MenuHelper.PrintStart("BINA.AZ BOT");
            int days = MenuHelper.GetDayChoice();
            DateTime endDate = DateTime.Today.AddDays(-days);

            int page = 1;
            await BinaAzHelper.StartInitialRun();

            string? cursor = null;
            Variables variables = new Variables() { first = 16, sort = "BUMPED_AT_DESC" };
            if (cursor != null)
            {
                variables.cursor = cursor;
            }
            Extensions extensions = new Extensions() { persistedQuery = new PersistedQuery() { version = 1, sha256Hash = "872e9c694c34b6674514d48e9dcf1b46241d3d79f365ddf20d138f18e74554c5" } };
            GraphqlQueryParams queryParams = new GraphqlQueryParams() { variables = variables, extensions = extensions, operationName = "SearchItems" };
            int currentItem = 1;
            Console.WriteLine("Starting to fetch datas");
            while (true)
            {
                var dataPage = await BinaAzHelper.GetData(queryParams);
                if (dataPage is null)
                    return;
                
                foreach (var item in dataPage.Data.ItemsConnection.Edges)
                {
                    Console.WriteLine($"---------------------- {currentItem++}/{page} ----------------------");
                    var property = item.Node.GetInitialProperty(); 
                    if (property.UpdatedTime < endDate)
                    {
                        Console.WriteLine("Data older than 2 days reached, stopping the process.");
                        return;
                    }
                    var existingRecord = database.FindById(property.Id);
                    if (existingRecord is not null)
                    {
                        Console.WriteLine($"Recording with this id ({property.Id}) exists");
                        continue;
                    }
                    var htmlString = await helper.GetPage($"/items/{property.Id}");
                    if (string.IsNullOrEmpty(htmlString))
                        throw new Exception("Html Content returned null or empty");
                    var contentProperty = await helper.GetPropertyFromRawHTML(htmlString, property);
                    contentProperty.PrintDetails();
                    database.InsertRecord(contentProperty);
                    Thread.Sleep(500);
                }
                page++;
                Thread.Sleep(1500);
            }
        }
    }
}
