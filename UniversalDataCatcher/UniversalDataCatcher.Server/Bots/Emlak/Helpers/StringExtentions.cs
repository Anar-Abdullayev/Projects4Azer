namespace UniversalDataCatcher.Server.Bots.Emlak.Helpers
{
    public static class StringExtentions
    {
        public static string GetAddressStringForEmlak(this string? value)
        {
            if (value is null)
                return "";
            int count = 0;
            int index = -1;

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    count++;
                    if (count == 2)
                    {
                        index = i;
                        break;
                    }
                }
            }
            if (index != -1)
            {
                string result = value.Substring(index);
                return result;
            }
            return value;
        }
    }
}
