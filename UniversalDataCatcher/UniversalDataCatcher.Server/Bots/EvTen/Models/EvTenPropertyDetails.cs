using System.Text.Json.Serialization;
using UniversalDataCatcher.Server.Bots.EvTen.StaticConstants;

namespace UniversalDataCatcher.Server.Bots.EvTen.Models
{
    public class EvTenPropertyDetails
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("created_by")]
        public int CreatedBy { get; set; }

        [JsonPropertyName("original_owner_id")]
        public int OriginalOwnerId { get; set; }

        [JsonPropertyName("last_modified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("owner_name")]
        public string OwnerName { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("floor")]
        public int Floor { get; set; }

        [JsonPropertyName("total_floors")]
        public int TotalFloors { get; set; }

        [JsonPropertyName("area")]
        public double? Area { get; set; }
        [JsonPropertyName("land_area")]
        public double? LandArea { get; set; }

        [JsonPropertyName("rooms")]
        public int Rooms { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("location_lat")]
        public double LocationLat { get; set; }

        [JsonPropertyName("location_lng")]
        public double LocationLng { get; set; }

        [JsonPropertyName("images")]
        public List<Image> Images { get; set; }

        [JsonPropertyName("for_sale")]
        public bool ForSale { get; set; }

        [JsonPropertyName("property_type")]
        public string PropertyType { get; set; }
        public string? Category
        {
            get
            {
                if (PropertyType == "apartment")
                    return "Mənzil";
                if (PropertyType == "office")
                    return "Ofis";
                if (PropertyType == "house")
                    return "Həyət evi";
                if (PropertyType == "commercial property")
                    return "Obyekt";
                if (PropertyType == "land")
                    return "Torpaq";
                if (PropertyType == "garage")
                    return "Qaraj";
                if (PropertyType is not null)
                    return PropertyType;
                return null;
            }
        }

        public string? BinaType
        {
            get
            {
                if (PropertyType == "apartment")
                {
                    if (IsNewBuilding)
                        return "Yeni tikili";
                    else
                        return "Köhnə tikili";
                }
                return null;
            }
        }

        [JsonPropertyName("is_agent")]
        public bool IsAgent { get; set; }
        public string PosterType { get { return IsAgent ? "vasitəçi" : "mülkiyyətçi"; } }
        [JsonPropertyName("has_license")]
        public bool? Has_License { get; set; }
        public string? Document { get { return Has_License is null ? null : (bool)Has_License ? "var" : "yox"; } }

        [JsonPropertyName("renovated")]
        public bool Renovated { get; set; }
        public string Renovation { get { return Renovated ? "təmirli" : "təmirsiz"; } }

        [JsonPropertyName("is_new_building")]
        public bool IsNewBuilding { get; set; }

        [JsonPropertyName("washroom_count")]
        public int WashroomCount { get; set; }

        [JsonPropertyName("amenities")]
        public List<string> Amenities { get; set; }

        [JsonPropertyName("renewed_at")]
        public DateTime RenewedAt { get; set; }

        [JsonPropertyName("approved_at")]
        public DateTime ApprovedAt { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("district")]
        public string District { get; set; }

        [JsonPropertyName("office_type")]
        public string OfficeType { get; set; }

        [JsonPropertyName("sale_price_statistics")]
        public List<object> SalePriceStatistics { get; set; }

        [JsonPropertyName("lease_price_statistics")]
        public List<object> LeasePriceStatistics { get; set; }

        [JsonPropertyName("nearest_bus_station")]
        public NearestBusStation NearestBusStation { get; set; }

        [JsonPropertyName("nearest_university")]
        public NearestUniversity NearestUniversity { get; set; }

        [JsonPropertyName("nearest_school")]
        public NearestSchool NearestSchool { get; set; }

        [JsonPropertyName("nearest_preschool")]
        public NearestPreschool NearestPreschool { get; set; }

        [JsonPropertyName("nearest_hospital")]
        public NearestHospital NearestHospital { get; set; }

        [JsonPropertyName("lease_type")]
        public string LeaseType { get; set; }

        [JsonPropertyName("sale_type")]
        public string SaleType { get; set; }
        public string PostType { get { return SaleType == "PURCHASE" ? "Satış" : SaleType == "LEASE" ? "Kirayə" : SaleType; } }

        [JsonPropertyName("subway_station")]
        public SubwayStation SubwayStation { get; set; }

        [JsonPropertyName("is_sponsor")]
        public bool IsSponsor { get; set; }

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; }

        [JsonPropertyName("price_history")]
        public List<PriceHistory> PriceHistory { get; set; }

        [JsonPropertyName("city_id")]
        public int CityId { get; set; }

        [JsonPropertyName("district_id")]
        public int DistrictId { get; set; }

        [JsonPropertyName("landmarks")]
        public List<Landmark> Landmarks { get; set; }

        [JsonPropertyName("average_review")]
        public double AverageReview { get; set; }

        [JsonPropertyName("number_of_reviews")]
        public int NumberOfReviews { get; set; }

        [JsonPropertyName("is_editable")]
        public bool IsEditable { get; set; }
        public string AdvLink { get { return EvTenConstants.EvTenItemBaseUrl + Id; } }
        public string? MainTitle { get; set; }
        public bool HasIpoteka { get; set; }
        public string? Ipoteka { get { return HasIpoteka ? "var" : null; } }
    }

    public class Image
    {
        [JsonPropertyName("high_quality_url")]
        public string HighQualityUrl { get; set; }

        [JsonPropertyName("medium_quality_url")]
        public string MediumQualityUrl { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonPropertyName("image_id")]
        public string ImageId { get; set; }

        [JsonPropertyName("phash")]
        public string Phash { get; set; }
    }

    public class NearestBusStation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance_meters")]
        public double DistanceMeters { get; set; }

        [JsonPropertyName("bus_names")]
        public List<string> BusNames { get; set; }
    }

    public class NearestUniversity
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance_meters")]
        public double DistanceMeters { get; set; }
    }

    public class NearestSchool
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance_meters")]
        public double DistanceMeters { get; set; }
    }

    public class NearestPreschool
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance_meters")]
        public double DistanceMeters { get; set; }
    }

    public class NearestHospital
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance_meters")]
        public double DistanceMeters { get; set; }
    }

    public class SubwayStation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance_meters")]
        public double DistanceMeters { get; set; }
    }

    public class PriceHistory
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }

    public class Landmark
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance_meters")]
        public double DistanceMeters { get; set; }
    }
}
