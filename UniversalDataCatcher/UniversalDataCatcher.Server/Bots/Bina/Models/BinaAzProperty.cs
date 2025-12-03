namespace UniversalDataCatcher.Server.Bots.Bina.Models
{
    public class BinaAzProperty
    {
        private string? _area;
        private string? _landArea;
        private string? _floor;
        private string? _phoneNumbers;
        private string? _ownerType;
        private string? _cixaris;
        private string? _category;
        private string? _renovation;


        public int Id { get; set; } // bina_id , item_id
        public string? MainTitle { get; set; } // main_title

        public string? Category { get { return _category is null ? null : _category.Contains("tikili") ? "Mənzil" : _category.Contains("Həyət evi") ? "Həyət evi" : _category; } set { _category = value; } }
        public string? Floor { get { return _floor?.Split("/")[0]; } set { _floor = value; } } // floor
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? FullAddress { get { return City + ", " + Address; } } // address
        public string? Area { get { return _area?.Split("m")[0].Replace("sot", ""); } set { _area = value; } } // area
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
        public string? BuildingType { get { return _category is not null && _category.Contains("tikili") ? _category : null; } }
        public string? Cixaris { get { return _cixaris is not null && _cixaris == "yoxdur" ? "yox" : _cixaris; } set { _cixaris = value; } } // cixaris
        public string? Repair { get { return _renovation is null ? null : _renovation == "yoxdur" ? "yox" : "var"; } set { _renovation = value; } } // repair
        public string? RoomCount { get; set; } // room
        public string? Price { get; set; } // amount
        public string? Currency { get; set; } // currency
        public string? Owner { get; set; } // poster_name
        public string? OwnerType
        {
            get
            {
                if (_ownerType is null)
                    return null;
                if (_ownerType.StartsWith("mülkiyyətçi"))
                    return "mülkiyyətçi";
                if (_ownerType.StartsWith("vasitəçi"))
                    return "vasitəçi";
                if (_ownerType.StartsWith("Yaşayış kompleksi"))    
                    return "vasitəçi";
                return _ownerType;
            }
            set { _ownerType = value; }
        } // poster_type
        //public string? OwnerType { get { return _ownerType is null ? null : _ownerType == "" ? "vasitəçi" : _ownerType?.Replace(" (agent)", ""); } set { _ownerType = value; } } // poster_type
        public string? PhoneNumbers { get { return _phoneNumbers?.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""); } set { _phoneNumbers = value; } }
        public string? Description { get; set; } // poster_note
        public string AdvLink { get; set; } // sayt_link
        public string? Post_Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedTime { get; set; } // updated
        public string? ImageUrls { get; set; }

    }
}
