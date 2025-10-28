using DataCatcherBot;
using HtmlAgilityPack;

namespace TapAzDataCatcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var tapazHelper = new TapazHelper();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var databaseService = new MSSqlDatabaseService();
            bool continueSearch = true;
            int page = 1;
            MenuHelper.PrintStart();
            int dayDifference = MenuHelper.GetDayChoice();
            DateTime targetDate = DateTime.Now.AddDays(-dayDifference);
            var tillDateString = MenuHelper.FormatAzeriDate(targetDate);
            var formattedDates = MenuHelper.GetFormattedDatesUntil(targetDate);
            Console.WriteLine($"Təsdiqlənmiş gün: {dayDifference} gün öncəyədək ({tillDateString} daxil deyil).");
            Console.WriteLine($"Başlayır...");
            string nextUrl = "";
            while (continueSearch)
            {
                var htmlContent = await tapazHelper.GetPage(nextUrl);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                nextUrl = DocumentHelper.GetNextPageUrl(doc);
                if (htmlContent != null)
                {
                    var propertyNodes = tapazHelper.GetPropertiesFromContent(doc, formattedDates, ref continueSearch);

                    if (propertyNodes != null)
                    {
                        Console.WriteLine($"Səhifə - {page}");
                        int row = 1;
                        foreach (var propertyNode in propertyNodes)
                        {
                            Console.Write($"{row++}/{page} - ");
                            var existingRecord = databaseService.FindById(int.Parse(propertyNode.Item1));
                            if (existingRecord != null)
                            {
                                Console.WriteLine($"ID-si {propertyNode.Item1} olan elan oxunulub. Ötürülür...");
                                continue;
                            }

                            var detailHtml = await tapazHelper.GetPage(propertyNode.Item2);
                            if (detailHtml == null)
                                continue;
                            var property = await tapazHelper.GetPropertyFromRawHTML(detailHtml, propertyNode.Item1);
                            property.Id = int.Parse(propertyNode.Item1);
                            property.AdvLink = Constants.BaseUrl + propertyNode.Item2;
                            databaseService.InsertRecord(property);
                            Console.WriteLine("--------------------------------");
                            Console.WriteLine($"Yeni elan tapıldı");
                            Console.WriteLine($"Elanın linki: {propertyNode.Item2}");
                            Console.WriteLine($"Elanın tarixi: {propertyNode.Item3}");
                            property.PrintDetails();
                            Console.WriteLine("--------------------------------");
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }
                }
                page++;
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("End of search bot");
        }
    }
}
