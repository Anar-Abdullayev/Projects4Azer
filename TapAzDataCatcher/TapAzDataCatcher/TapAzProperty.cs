using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TapAzDataCatcher
{
    public class TapAzProperty
    {
        private string _price;
        private string _advType;
        public int Id { get; set; }
        public string MainTitle { get; set; }


        public string City { get; set; } // Şəhər
        public string Address { get; set; } // Yerləşdirmə yeri / Yerləşmə yeri
        public string AdvType { get { return string.IsNullOrEmpty(_advType) ? "Satış" : _advType; } set { _advType = value; } } // Elanın tipi
        public string PropType { get; set; } // Əmlakın növü
        public string Area { get; set; } // Sahə
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

        public string Price { get { return _price; } set { _price = value.Replace(" AZN", "").Replace(" ", ","); } }
        public string Owner { get; set; }
        public string OwnerType { get; set; }
        public string PhoneNumbers { get; set; }

        public string Description { get; set; }

        public string AdvLink { get; set; }
        public void PrintDetails()
        {
            Console.WriteLine("Elanın detalları:");
            Console.WriteLine("İd: " + Id);
            Console.WriteLine("Başlıq: " + MainTitle);
            Console.WriteLine("Şəhər: " + City);
            Console.WriteLine("Yerləşmə yeri: " + Address);
            Console.WriteLine("Elanın tipi: " + AdvType);
            Console.WriteLine("Əmlakın növü: " + PropType);
            Console.WriteLine("Sahə: " + Area);
            Console.WriteLine("Kirayə müddəti: " + RentLong);
            Console.WriteLine("Binanın tipi: " + BuildingType);
            Console.WriteLine("Otaq sayı: " + RoomCount);
            Console.WriteLine("Qiymət: " + Price);
            Console.WriteLine("Sahibi: " + Owner);
            Console.WriteLine("Əlaqə nömrələri: " + PhoneNumbers);
            Console.WriteLine("Təsvir: " + Description);
        }
    }
}
