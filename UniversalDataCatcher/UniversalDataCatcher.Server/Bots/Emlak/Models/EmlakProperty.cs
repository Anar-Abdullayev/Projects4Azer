namespace UniversalDataCatcher.Server.Bots.Emlak.Models
{
    public class EmlakProperty
    {
        private string? _document;
        private string? _category;
        private string? _posterType;
        private string? _posterPhone;
        public int Id { get; set; }
        public string AdvLink { get; set; }
        public string MainTitle { get; set; }
        public string Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Category
        {
            get
            {
                var value = _category;
                if (value is not null)
                {
                    if (value == "Köhnə tikili" || value == "Yeni tikili")
                        return "Mənzil";
                    else if (value == "Bağ evi" || value == "Villa")
                        return "Həyət evi";
                }
                return value;
            }
            set { _category = value; }
        }
        public string? BinaType
        {
            get
            {
                if (_category == "Köhnə tikili" || _category == "Yeni tikili")
                    return _category;
                return null;
            }
        }
        public string? Area { get; set; }
        public string? LandArea { get; set; }
        public string? Rooms { get; set; }
        public string? Floor { get; set; }
        public string? ApartmentFloor { get; set; }
        public string? Renovation { get; set; }
        public string? Document { get { return _document is not null && _document.Contains("Kupça") ? "var" : null; } set { _document = value; } }
        public string? PosterName { get; set; }
        public string? PosterPhone { get { return _posterPhone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""); } set { _posterPhone = value; } }
        public string? PosterType { get { return _posterType.Replace("(","").Replace(")","").ToLower(); } set { _posterType = value; } }
        public string? Address { get; set; }
        public string PostType { get { return MainTitle.StartsWith("Satılır") ? "Satış" : MainTitle.StartsWith("İcarəyə") ? "Kirayə" : MainTitle.Split(" ")[0]; } }
    }
}
