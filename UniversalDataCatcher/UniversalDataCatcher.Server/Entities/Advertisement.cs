namespace UniversalDataCatcher.Server.Entities
{
    public class Advertisement
    {
        public int Id { get; set; }
        public int? Bina_Id { get; set; }
        public string? Main_Title { get; set; }
        public string? Address { get; set; }
        public string? Area { get; set; }
        public string? TorpaqArea { get; set; }
        public string? Amount { get; set; }
        public string? Currency { get; set; } = "AZN";
        public string? Renovation { get; set; }
        public string? Document { get; set; }
        public string? Ipoteka { get; set; }
        public string? BinaType { get; set; }
        public string? Room { get; set; }
        public string? Floor { get; set; }
        public string? Category { get; set; }
        public string? Item_Id { get; set; }
        public string? Poster_Name { get; set; }
        public string? Poster_Note { get; set; }
        public string? Post_Tip { get; set; }
        public string? Poster_Type { get; set; }
        public string? Poster_Phone { get; set; }
        public DateTime? Post_Create_Date { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime Updated { get; set; }
        public string? Sayt { get; set; }
        public string? Sayt_Link { get; set; }
        public DateTime? Status { get; set; }
        public int? ReferenceId { get; set; }
        public string? ImageUrls { get; set; }

    }
}
