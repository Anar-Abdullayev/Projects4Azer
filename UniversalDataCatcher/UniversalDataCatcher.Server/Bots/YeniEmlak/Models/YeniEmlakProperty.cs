namespace UniversalDataCatcher.Server.Bots.YeniEmlak.Models
{
    public class YeniEmlakProperty
    {
        private string? _postType;
        private string? _posterPhone;


        public int Id { get; set; }
        public string? Price { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AdvLink { get; set; }
        public string? Rooms { get; set; }
        public string? Area { get; set; }
        public string? LandArea { get; set; }
        public string? Floor { get; set; }
        public string? Document { get; set; }
        public string? BinaType { get; set; }
        public string? Category { get; set; }
        public string? PosterName { get; set; }
        public string? PosterType { get; set; }
        public string? Renovation { get; set; }


        public string? PosterPhone { get { return _posterPhone; } set { _posterPhone = value; } }
        public string? PostType { get { return _postType == "Satılır" ? "Satış" : _postType; } set { _postType = value; } }
    }
}
