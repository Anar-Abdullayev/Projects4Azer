namespace UniversalDataCatcher.Server.Services.Arenda.Model
{
    public class ArendaProperty
    {
        public string Id { get; set; } 
        public string? MainTitle { get; set; }
        public string? SecondaryTitle { get; set; } 
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
