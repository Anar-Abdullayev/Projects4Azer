using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.Text.Json.Serialization;
using UniversalDataCatcher.Server.Bots.Lalafo.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Models
{
    public class LalafoProperty
    {
        private string? _url;
        private string? _mobile;
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; } = "";
        [JsonPropertyName("price")]
        public float? Price { get; set; } = 0;
        [JsonPropertyName("city")]
        public string? City { get; set; } = "";
        [JsonPropertyName("description")]
        public string? Description { get; set; } = "";
        [JsonPropertyName("username")]
        public string? Username { get; set; } = "";
        [JsonPropertyName("mobile")]
        public string? Mobile { get { return _mobile?.Replace("+994", "0"); } set { _mobile = value; } }
        [JsonPropertyName("created_time")]
        public long CreatedTime { get; set; }
        [JsonPropertyName("updated_time")]
        public long UpdatedTime { get; set; }
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }
        [JsonPropertyName("is_vip")]
        public bool? IsVip { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get { return "https://lalafo.az" + _url; } set { _url = value; } }
        [JsonPropertyName("ad_label")]
        public string? Ad_Label { get; set; }

        public string? Post_Type { get { return Ad_Label.Contains("satılır") ? "Satış" : Ad_Label.Contains("kirayə") ? "Kirayə" : null; } }

        public string Address
        {
            get
            {
                var metro = Parameters.FirstOrDefault(x => x.Id == 359);
                var inzibatiRayon = Parameters.FirstOrDefault(x => x.Id == 358);
                var rayon = Parameters.FirstOrDefault(x => x.Id == 1161);
                var address = City + " " + inzibatiRayon?.Value + " " + rayon?.Value + " " + metro?.Value;
                return address;
            }
        }

        public string? PropertyArea
        {
            get
            {
                var area = Parameters.FirstOrDefault(x => x.Id == 70);
                return area?.Value;
            }
        }

        public string? LandArea { get { var landArea = Parameters.FirstOrDefault(x => x.Id == 71); return landArea?.Value; } }

        public string? RoomCount { get { var rooms = Parameters.FirstOrDefault(x => x.Id == 69); return rooms?.Value.Replace(" otaqlı", ""); } }

        public string? BuildingType { get { return CategoryHelper.GetBuildingTypeName(CategoryId); } }
        public string? Category { get { return CategoryHelper.GetCategoryName(CategoryId); } }
        public string? Floor { get { var floor = Parameters.FirstOrDefault(x => x.Id == 226); return floor?.Value; } }
        public string? Document { get { var document = Parameters.FirstOrDefault(x => x.Id == 888); return document is not null ? "var" : null; } }
        public string? Repair { get { var repair = Parameters.FirstOrDefault(x => x.Id == 352); return repair is not null ? repair.Value : "yoxdur"; } }
        public string? Poster_Type
        {
            get
            {
                var poster = Parameters.FirstOrDefault(x => x.Value_id == 19055 || x.Value_id == 19057);
                if (poster is null)
                    return "Bilinmir";
                return poster.Value_id == 19057 ? "mülkiyyətçi" : "vasitəçi";
            }
        }

        [JsonPropertyName("images")]
        public List<Image> Images { get; set; } = new();
        [JsonPropertyName("params")]
        public List<LalafoParams> Parameters { get; set; } = new();
    }
    public class LalafoParams
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; } = "";
        [JsonPropertyName("value")]
        public string? Value { get; set; } = "";
        [JsonPropertyName("value_id")]
        public int? Value_id { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("original_url")]
        public string? OriginalUrl { get; set; } = "";
    }

    public class LalafoParamLink
    {

    }
}


