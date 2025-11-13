using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCatcherBot
{
    public static class MenuHelper
    {
        public static void PrintStart()
        {
            string title = "ARRENDA.AZ BOT";
            int padding = 6;
            int width = title.Length + padding * 2;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('*', width));
            Console.WriteLine("*" + new string(' ', width - 2) + "*");
            Console.WriteLine("*" + new string(' ', padding - 1) + title + new string(' ', padding - 1) + "*");
            Console.WriteLine("*" + new string(' ', width - 2) + "*");
            Console.WriteLine(new string('*', width));
            Console.ResetColor();
        }

        public static int GetDayChoice()
        {
            while (true)
            {
                Console.Write("Neçə gün fərq ilə axtarılsın? (0 - bugün, 1 - dünən, və s.): ");
                string input = Console.ReadLine();
                if (!int.TryParse(input, out int days))
                {
                    Console.WriteLine("Sadəcə rəqəm daxil edə bilərsən.");
                    continue;
                }
                DateTime targetDate = DateTime.Today.AddDays(-days);
                string formattedDate = targetDate.ToString("dd.MM.yyyy");
                Console.Write($"Bugündən {formattedDate} tarixədək olan dataları çəksin? (y - hə/n - yox): ");
                string confirmation = Console.ReadLine().Trim().ToLower();
                if (confirmation == "yes" || confirmation == "y")
                {
                    return days;
                }
                Console.WriteLine("Yenidən cəhd edək...");
            }
        }

        public static string FormatAzeriDate(DateTime date)
        {
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);
            date = date.AddDays(-1);
            if (date.Date == today.Date)
                return "Bugün";
            else if (date.Date == yesterday.Date)
                return "Dünən";
            else
            {
                string[] months = {
                "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
                "İyul", "Avqust", "Sentyabr", "Oktyabr", "Dekabr"
            };

                int day = date.Day;
                string month = months[date.Month - 1];
                int year = date.Year;

                return $"{day:00} {month} {year}";
            }
        }

        public static List<string> GetFormattedDatesUntil(DateTime targetDate)
        {
            var formattedDates = new List<string>()
            {
                "Bugün"
            };
            DateTime today = DateTime.Today;

            for (DateTime d = today; d > targetDate.Date; d = d.AddDays(-1))
            {
                formattedDates.Add(FormatAzeriDate(d));
            }

            return formattedDates;
        }
    }
}
