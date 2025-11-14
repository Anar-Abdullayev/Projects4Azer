namespace UniversalDataCatcher.Server.Services.Arenda.Helpers
{
    public static class FormatHelper
    {
        private static string[] months = {
            "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
            "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
        };
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
                int day = date.Day;
                string month = months[date.Month - 1];
                int year = date.Year;

                return $"{day:00} {month} {year}";
            }
        }

        public static DateTime ParseAzeriDateWithTime(string str)
        {
            string datePart = str;
            string timePart = "00:00"; // default time if not provided

            if (str.Contains(","))
            {
                var parts = str.Split(',');
                datePart = parts[0].Trim();
                timePart = parts[1].Trim();
            }

            int hour = 0, minute = 0;
            if (TimeSpan.TryParse(timePart, out TimeSpan ts))
            {
                hour = ts.Hours;
                minute = ts.Minutes;
            }

            DateTime baseDate;
            if (datePart == "Bugün")
                baseDate = DateTime.Today;
            else if (datePart == "Dünən")
                baseDate = DateTime.Today.AddDays(-1);
            else
            {
                var dateParts = datePart.Split(' ');
                if (dateParts.Length != 3)
                    throw new FormatException("Invalid Azeri date format.");

                int day = int.Parse(dateParts[0]);
                string monthName = dateParts[1];
                int year = int.Parse(dateParts[2]);

                int month = Array.IndexOf(months, monthName) + 1;
                if (month == 0)
                    throw new FormatException($"Invalid month name: {monthName}");

                baseDate = new DateTime(year, month, day);
            }

            return baseDate.AddHours(hour).AddMinutes(minute);
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
