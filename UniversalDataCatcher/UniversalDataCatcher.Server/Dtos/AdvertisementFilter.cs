namespace UniversalDataCatcher.Server.Dtos
{
    public class AdvertisementFilter
    {
        public string? Category { get; set; }
        public string? BuildingType { get; set; }
        public string? PostType { get; set; }
        public string? Poster_Type { get; set; }
        public List<string>? City { get; set; }
        public bool HideRepeats { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
