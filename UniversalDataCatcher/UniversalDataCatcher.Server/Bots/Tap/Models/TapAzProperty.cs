using System.Globalization;

namespace UniversalDataCatcher.Server.Bots.Tap.Models
{
    public class TapAzProperty
    {
        private string _phoneNumbers;
        private string _price;
        private string _advType;
        public int Id { get; set; }
        public string MainTitle { get; set; }


        public string? City { get; set; }
        public string? Address { get; set; }
        public string? FinalAddress
        {
            get
            {
                return $"{City}, {Address}";
            }
        }
        public string AdvType
        {
            get
            {
                string type = _advType;
                if (Category == "Torpaq")
                    type = "Satış";
                else if (RentLong == "Günlük")
                    type = "Günlük";
                else if (RentLong == "Aylıq")
                    type = "Kirayə";
                else if (string.IsNullOrEmpty(_advType))
                    type = _advType;
                else if (_advType == "Satış" || _advType == "Satılır")
                    type = "Satış";
                else if (_advType.Contains("Kirayə") || _advType.Contains("İcarəyə"))
                    type = "Kirayə";
                return type;
            }
            set { _advType = value; }
        } // Elanın tipi
        public string PropType { get; set; } // Əmlakın növü
        public string Area { get; set; } // Sahə
        public string LandArea { get; set; } // Sot
        public string RentLong { get; set; }
        public string BuildingType { get; set; } // Binanın tipi
        public string RoomCount { get; set; } // Otaq sayı
        public string Category
        {
            get
            {
                string category = "Digər";
                if (!string.IsNullOrEmpty(BuildingType))
                    category = "Mənzil";
                else if (!string.IsNullOrEmpty(PropType))
                {
                    if (PropType == "Villa" || PropType.Contains("Bağ"))
                        category = "Həyət evi";
                    else
                        category = PropType;
                }
                else if (MainTitle.Contains("torpaq"))
                    category = "Torpaq";
                return category;
            }
        }

        public string Price { get { return _price; } set { _price = value.Replace(" AZN", "").Replace(" ", ""); } }
        public string Owner { get; set; }
        public string OwnerType { get; set; }
        public string? PhoneNumbers { get { return _phoneNumbers?.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""); } set { _phoneNumbers = value; } }

        public string Description { get; set; }

        public string AdvLink { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
