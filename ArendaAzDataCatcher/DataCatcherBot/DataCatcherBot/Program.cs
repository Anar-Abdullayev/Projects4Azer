using System.Runtime.CompilerServices;

namespace DataCatcherBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
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
            while (continueSearch)
            {
                var htmlContent = await ArendaHelper.GetPage(page);
                if (htmlContent != null)
                {
                    var propertyNodes = ArendaHelper.GetPropertiesFromContent(htmlContent, formattedDates, ref continueSearch);
                    if (propertyNodes != null)
                    {
                        Console.WriteLine($"Səhifə - {page}");
                        int row = 1;
                        foreach (var propertyNode in propertyNodes)
                        {
                            Console.Write($"{row++}/{page} - ");
                            var existingRecord = databaseService.FindById(propertyNode.Item1);
                            if (existingRecord != null)
                            {
                                Console.WriteLine($"ID-si {propertyNode.Item1} olan elan oxunulub. Ötürülür...");
                                continue;
                            }

                            var detailHtml = await ArendaHelper.GetPropertyDetailPage(propertyNode.Item2);
                            if (detailHtml == null)
                                continue;
                            var property = ArendaHelper.GetPropertyFromRawHTML(detailHtml);
                            property.Id = propertyNode.Item1;
                            databaseService.InsertRecord(property);
                            Console.WriteLine("--------------------------------");
                            Console.WriteLine($"Yeni elan tapıldı");
                            Console.WriteLine($"Elanın linki: {propertyNode.Item2}");
                            Console.WriteLine($"Elanın tarixi: {propertyNode.Item3}");
                            property.PrintDetails();
                            Console.WriteLine("--------------------------------");
                            await Task.Delay(TimeSpan.FromSeconds(2));
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
