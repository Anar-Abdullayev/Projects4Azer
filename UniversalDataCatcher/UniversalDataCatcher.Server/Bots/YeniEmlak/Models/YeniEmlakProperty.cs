namespace UniversalDataCatcher.Server.Bots.YeniEmlak.Models
{
    public class YeniEmlakProperty
    {
        private string? _postType;
        private string? _posterPhone;
        private string? _binaType;
        private string? _category;
        private string? _renovation;


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
        public string? Category { get { return string.IsNullOrEmpty(_category) ? null : _category; } set { _category = value; } } // Binatype
        public string? PosterName { get; set; }
        public string? PosterType { get; set; }
        public string? Renovation { get { return _renovation is null ? null : _renovation == "təmirli" ? "var" : null; } set { _renovation = value; } }


        public string? BinaType // Category
        {
            get
            {
                string? type = _binaType;
                if (string.IsNullOrWhiteSpace(_binaType))
                    type = _binaType;
                else if (_binaType.Contains("Torpaq"))
                    type = "Torpaq";
                else if (_binaType.Contains("Həyət evi"))
                    type = "Həyət evi";
                else if (_binaType.Contains("Bina evi"))
                    type = "Mənzil";
                return type;
            }
            set { _binaType = value; }
        }

        public string? PosterPhone { get { return _posterPhone; } set { _posterPhone = value; } }
        public string? PostType { get { return _postType == "Satılır" ? "Satış" : _postType; } set { _postType = value; } }
        public string? ImageUrls { get; set; }
    }
}
