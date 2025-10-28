using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaAzDataCatcher.Models
{

    public class BinaAzProperty
    {
        private string _advType;


        public int Id { get; set; } // bina_id , item_id
        public string? MainTitle { get; set; } // main_title

        public string? Category { get; set; } // category
        public string? Floor { get; set; } // floor
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? FullAddress { get { return City + ", " + Address; } } // address
        public string? Area { get; set; } // area
        public string? LandArea { get; set; } // landarea
        public string? Ipoteka { get; set; } // ipoteka
        public string? RentLong { get; set; } // post_tip
        public string? BuildingType { get; set; } // building_type
        public string? Cixaris { get; set; } // cixaris
        public string? Repair { get; set; } // repair
        public string? RoomCount { get; set; } // room
        public string? Price { get; set; } // amount
        public string? Currency { get; set; } // currency
        public string? Owner { get; set; } // poster_name
        public string? OwnerType { get; set; } // poster_type
        public string? PhoneNumbers { get; set; } // poster_phone
        public string? Description { get; set; } // poster_note
        public string AdvLink { get; set; } // sayt_link
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
