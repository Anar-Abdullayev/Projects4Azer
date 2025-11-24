namespace UniversalDataCatcher.Server.Services.Arenda.Model
{
    public class ArendaProperty
    {
        public string Id { get; set; }
        public string? MainTitle { get; set; }
        public string? SecondaryTitle { get; set; }
        public string? BinaType
        {
            get
            {
                if (SecondaryTitle is not null)
                {
                    if (SecondaryTitle.Contains("Yeni Tikili"))
                        return "Yeni Tikili";
                    else if (SecondaryTitle.Contains("Köhnə tikili"))
                        return "Köhnə tikili";
                }
                return null;
            }
        }
        public string? Category
        {
            get
            {
                if (SecondaryTitle is not null)
                {
                    if (SecondaryTitle.Contains("A frame") || SecondaryTitle.Contains("Bağ evi") || SecondaryTitle.Contains("Həyət evi"))
                        return "Həyət evi";
                    if (SecondaryTitle.ToLower().Contains("köhnə tikili") || SecondaryTitle.ToLower().Contains("yeni tikili"))
                        return "Mənzil";
                }
                return SecondaryTitle;
            }
        }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public float? PropertySize { get; set; }
        public string? TorpaqArea { get; set; }
        public int? RoomCount { get; set; }
        public string? Floor { get; set; }
        public string? ContactNumbers { get; set; }
        public string? Owner { get; set; }
        public List<string>? PropertyFeatures { get; set; }
        public List<string>? PropertyMainInfos { get; set; }
        public string? Link { get; set; }
        public DateTime? Created_At { get; set; }
        public string? Poster_Type { get; set; }
        public string? Post_Type { get; set; }
        public string? Document { get; set; }
        public string? Repair { get; set; }
    }
}
