namespace UniversalDataCatcher.Server.Bots.Tap.Models
{
    public class TapAzProperty
    {
        private string _phoneNumbers;
        private string _price;
        private string _advType;
        public int Id { get; set; }
        public string MainTitle { get; set; }


        public string City { get; set; } // Şəhər
        public string Address { get; set; } // Yerləşdirmə yeri / Yerləşmə yeri
        public string AdvType { get { return string.IsNullOrEmpty(_advType) ? "Satış" : _advType; } set { _advType = value; } } // Elanın tipi
        public string PropType { get; set; } // Əmlakın növü
        public string Area { get; set; } // Sahə
        public string LandArea { get; set; } // Sot
        public string RentLong { get; set; } // Kirayə müddəti
        public string BuildingType { get; set; } // Binanın tipi
        public string RoomCount { get; set; } // Otaq sayı
        public string Category
        {
            get
            {
                var category = !string.IsNullOrEmpty(BuildingType) ? BuildingType : !string.IsNullOrEmpty(PropType) ? PropType : MainTitle.Contains("torpaq") ? "Torpaq" : "Digər";
                return category;
            }
        }

        public string Price { get { return _price; } set { _price = value.Replace(" AZN", "").Replace(" ", ""); } }
        public string Owner { get; set; }
        public string OwnerType { get; set; }
        public string? PhoneNumbers { get { return _phoneNumbers?.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-",""); } set { _phoneNumbers = value; } }

        public string Description { get; set; }

        public string AdvLink { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
