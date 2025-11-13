namespace UniversalDataCatcher.Server.Services.Arenda.Helpers
{
    public static class FormatHelper
    {
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
