namespace UniversalDataCatcher.Server.Bots.Emlak.Models
{
    public class EmlakProperty
    {
        private string? _document;
        public int Id { get; set; }
        public string AdvLink { get; set; }
        public string MainTitle { get; set; }
        public string Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Area { get; set; }
        public string? LandArea { get; set; }
        public string? Rooms { get; set; }
        public string? Floor { get; set; }
        public string? ApartmentFloor { get; set; }
        public string? Renovation { get; set; }
        public string? Document { get { return _document is not null && _document.Contains("Kupça") ? "var" : null; } set { _document = value; } }
        public string? PosterName { get; set; }
        public string? PosterPhone { get; set; }
        public string? PosterType { get; set; }
        public string? Address { get; set; }
        public string PostType { get { return MainTitle.StartsWith("Satılır") ? "Satış" : MainTitle.StartsWith("İcarəyə") ? "Kirayə" : MainTitle.Split(" ")[0]; } }
    }
}
