using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace UniversalDataCatcher.Server.Bots.Lalafo.Models
{
    public class LalafoProperty
    {
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
        public string? Username { get; set; } = "";
        [JsonPropertyName("mobile")]
        public string? Mobile { get; set; } = "";
        [JsonPropertyName("created_time")]
        public long CreatedTime { get; set; }
        [JsonPropertyName("updated_time")]
        public long UpdatedTime { get; set; }
        [JsonPropertyName("is_vip")]
        public bool? IsVip { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; } = "";


        [JsonPropertyName("images")]
        public List<Image> Images { get; set; } = new();
        [JsonPropertyName("params")]
        public List<LalafoParams> Parameters { get; set; } = new();
    }
    public class LalafoParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; } = "";
        [JsonPropertyName("value")]
        public string? Value { get; set; } = "";
    }

    public class Image
    {
        [JsonPropertyName("original_url")]
        public string? OriginalUrl { get; set; } = "";
    }
}


