using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCatcherBot
{
    public class ArendaProperty
    {
        public string Id { get; set; } // id
        public string MainTitle { get; set; } //full elan_in_title_link elan_main_title -> h2 -> text.trim() (elan_desc_sides elan_desc_left_side)
        public string? SecondaryTitle { get; set; } // full elan_in_title_link -> h3 -> text.trim() (elan_desc_sides elan_desc_left_side)
        public string Address { get; set; } // elan_unvan_txt -> span -> text.trim() (elan_desc_sides elan_desc_left_side)
        public string Description { get; set; } // full elan_info_txt -> div -> p -> text.trim() (elan_desc_sides elan_desc_left_side)
        public float Price { get; set; } // full elan_new_price_box text-center -> div -> p -> text.trim() (elan_desc_sides elan_desc_right_side elan_in_right)
        public float? PropertySize { get; set; } // cerez
        public int? RoomCount { get; set; } // cerez
        public List<string>? ContactNumbers { get; set; } // elan_in_tel_box -> p -> many a elan_in_tel -> text.trim() (elan_desc_sides elan_desc_right_side elan_in_right) 
        public string Owner { get; set; } // new_elan_user_info full -> div -> first p -> text.trim() (elan_desc_sides elan_desc_right_side elan_in_right)
        public List<string>? PropertyFeatures { get; set; } // full property_lists -> ul -> li -> text.trim() (elan_desc_sides elan_desc_left_side)
        public List<string>? PropertyMainInfos { get; set; } // full elan_property_list -> ul -> li -> a -> text.trim() (elan_desc_sides elan_desc_left_side)

        public void PrintDetails()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Əsas başlıq: {MainTitle} | İkinci dərəcəli başlıq: {SecondaryTitle}");
            Console.WriteLine($"Ünvan: {Address}");
            Console.WriteLine($"Text: {Description}");
            Console.WriteLine($"Qiymət: {Price} AZN");
            Console.WriteLine($"Sahəsi: {PropertySize}");
            Console.WriteLine($"Otaq: {RoomCount}");
            Console.WriteLine("Elanın əsas informasiyaları");
            if (PropertyMainInfos is not null)
                foreach (var feature in PropertyMainInfos)
                    Console.WriteLine($" - {feature}");
            else
                Console.WriteLine(" - Yoxdur");

            Console.WriteLine("Elanın xüsusiyyətləri");
            if (PropertyFeatures is not null)
                foreach (var feature in PropertyFeatures)
                    Console.WriteLine($" - {feature}");
            else
                Console.WriteLine(" - Yoxdur");

            Console.WriteLine($"Sahibi: {Owner}");

            Console.WriteLine("Əlaqə nömrələri: ");
            if (ContactNumbers is not null)
                foreach (var numbers in ContactNumbers)
                    Console.WriteLine($" - {numbers}");
            else
                Console.WriteLine(" - Yoxdur");
        }
    }
}

// (elan_desc_sides elan_desc_left_side) section
// (elan_desc_sides elan_desc_right_side elan_in_right) section