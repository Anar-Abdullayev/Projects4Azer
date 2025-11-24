namespace UniversalDataCatcher.Server.Bots.Lalafo.Helpers
{
    public static class CategoryHelper
    {
        public static string GetCategoryName(int categoryId)
        {
            switch (categoryId)
            {
                case 2031:
                case 2033:
                case 2034:
                case 5952:
                case 5953:
                    return "Həyət evi";
                case 2038:
                case 2039:
                    return "Torpaq";
                case 2041:
                case 2044:
                case 2045:
                case 2050:
                case 2052:
                case 2053:
                case 4793:
                case 4800:
                    return "Mənzil";
                case 2055:
                case 2065:
                case 2066:
                case 2067:
                case 2069:
                case 2070:
                case 2071:
                case 2072:
                    return "Obyekt";
                case 2068:
                    return "Ofis";
                case 5304:
                case 5305:
                    return "Qaraj";
                case 2035:
                case 2042:
                    return "Digər";
                default:
                    return "Digər";
            }
        }

        public static string? GetBuildingTypeName(int categoryId)
        {
            switch (categoryId)
            {
                case 4793:
                    return "Yeni tikili";
                case 4800:
                    return "Köhnə tikili";
                default:
                    return null;
            }
        }

    }
}
