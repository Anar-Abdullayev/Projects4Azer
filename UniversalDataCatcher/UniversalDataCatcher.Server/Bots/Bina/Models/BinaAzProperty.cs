namespace UniversalDataCatcher.Server.Bots.Bina.Models
{
    public class BinaAzProperty
    {
        private string _advType;
        private string? _area;
        private string? _landArea;
        private string? _floor;
        private string? _phoneNumbers;
        private string? _ownerType;


        public int Id { get; set; } // bina_id , item_id
        public string? MainTitle { get; set; } // main_title

        public string? Category { get; set; } // category
        public string? Floor { get { return _floor?.Split("/")[0]; } set { _floor = value; } } // floor
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? FullAddress { get { return City + ", " + Address; } } // address
        public string? Area { get { return _area?.Split("m")[0].Replace("sot",""); } set { _area = value; } } // area
        public string? LandArea
        {
            get
            {
                if (!string.IsNullOrEmpty(_area) && _area.Contains("sot"))
                {
                    return _area.Replace("sot", "");
                }
                return _landArea?.Replace(" sot", "");
            }
            set { _landArea = value; }
        } // landarea
        public string? Ipoteka { get; set; }  // ipoteka
        public string? RentLong { get; set; } // post_tip
        public string? BuildingType { get; set; } // building_type
        public string? Cixaris { get; set; } // cixaris
        public string? Repair { get; set; } // repair
        public string? RoomCount { get; set; } // room
        public string? Price { get; set; } // amount
        public string? Currency { get; set; } // currency
        public string? Owner { get; set; } // poster_name
        public string? OwnerType { get { return _ownerType?.Replace(" (agent)", ""); } set { _ownerType = value; } } // poster_type
        public string? PhoneNumbers { get { return _phoneNumbers?.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""); } set { _phoneNumbers = value; } }
        public string? Description { get; set; } // poster_note
        public string AdvLink { get; set; } // sayt_link
        public string? Post_Type { get; set; }
        public string? CreatedAt { get; set; }
        public DateTime? UpdatedTime { get; set; } // updated

        public void PrintDetails()
        {
            Console.WriteLine("Elanın detalları:");
            Console.WriteLine("İd: " + Id);
            Console.WriteLine("Tarix: " + UpdatedTime.ToString());
            Console.WriteLine("Başlıq: " + MainTitle);
            Console.WriteLine("Şəhər: " + City);
            Console.WriteLine("Yerləşmə yeri: " + Address);
            Console.WriteLine("Sahə: " + Area);
            Console.WriteLine("Kirayə müddəti: " + RentLong);
            Console.WriteLine("Binanın tipi: " + BuildingType);
            Console.WriteLine("Otaq sayı: " + RoomCount);
            Console.WriteLine("Qiymət: " + Price + " " + Currency);
            Console.WriteLine("Sahibi: " + Owner + $" ({OwnerType})");
            Console.WriteLine("Əlaqə nömrələri: " + PhoneNumbers);
            Console.WriteLine("Detallı: " + Description);
            Console.WriteLine("Link: " + AdvLink);
        }
    }
}
