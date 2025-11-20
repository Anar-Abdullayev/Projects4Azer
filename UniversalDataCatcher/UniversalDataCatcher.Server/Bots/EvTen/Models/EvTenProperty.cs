using System.Text.Json.Serialization;

namespace UniversalDataCatcher.Server.Bots.EvTen.Models
{
    public class EvTenProperty
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("area")]
        public double Area { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("location_lat")]
        public double LocationLat { get; set; }

        [JsonPropertyName("location_lng")]
        public double LocationLng { get; set; }

        [JsonPropertyName("images")]
        public List<string> Images { get; set; } = new();

        [JsonPropertyName("property_type")]
        public string PropertyType { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("district")]
        public string District { get; set; } = string.Empty;

        [JsonPropertyName("suburban")]
        public string Suburban { get; set; } = string.Empty;

        [JsonPropertyName("renewed_at")]
        public DateTime RenewedAt { get; set; }

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; } = string.Empty;

        [JsonPropertyName("renovated")]
        public bool Renovated { get; set; }

        [JsonPropertyName("sale_type")]
        public string SaleType { get; set; } = string.Empty;

        [JsonPropertyName("subway_station")]
        public SubwayStation SubwayStation { get; set; } = new();

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("created_by")]
        public int CreatedBy { get; set; }

        [JsonPropertyName("has_mortgage")]
        public bool HasMortgage { get; set; }

        [JsonPropertyName("months_to_pay")]
        public int MonthsToPay { get; set; }

        [JsonPropertyName("average_review")]
        public double AverageReview { get; set; }

        [JsonPropertyName("number_of_reviews")]
        public int NumberOfReviews { get; set; }
    }

}
