using System.Text.Json.Serialization;

namespace UniversalDataCatcher.Server.Bots.Bina.Models
{
    public class BinaAzPropertyDetails
    {
        public string? __typename { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("address")]
        public string? AddressStreet { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("leased")]
        public bool PostTypeDetails { get; set; } // true - kirayə, false - satılır
        [JsonPropertyName("rooms")]
        public int RoomCount { get; set; }
        [JsonPropertyName("floor")]
        public int Floor { get; set; }
        [JsonPropertyName("metaTags")]
        public List<MetaTag>? MetaTags { get; set; }
        [JsonPropertyName("hasBillOfSale")]
        public bool DocumentDetails { get; set; }
        [JsonPropertyName("hasMortgage")]
        public bool IpotekaDetails { get; set; }
        [JsonPropertyName("hasRepair")]
        public bool RepairDetails { get; set; }
        [JsonPropertyName("contactName")]
        public string? PosterName { get; set; }
        [JsonPropertyName("contactTypeName")]
        public string? PosterTypeDetails { get; set; }
        public string? buildingTypeName { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string? path { get; set; }
        public object? business { get; set; }
        public List<NearestLocation>? nearestLocations { get; set; }
        public string? updatedAt { get; set; }
        public string? expiresAt { get; set; }
        public int views { get; set; }
        public bool isFeatured { get; set; }
        public bool isVipped { get; set; }
        public object? rejectReason { get; set; }
        public Location? location { get; set; }
        public object? landArea { get; set; }
        public Area? area { get; set; }
        public Price? price { get; set; }
        public object? paidDaily { get; set; }
        public Company? company { get; set; }
        public Category? category { get; set; }
        public City? city { get; set; }
        public List<Photo>? photos { get; set; }
    }
    public class Breadcrumb
    {
        public string? __typename { get; set; }
        public string? name { get; set; }
        public string? path { get; set; }
    }

    public class MetaTag
    {
        public string? __typename { get; set; }
        public string? name { get; set; }
        public string? content { get; set; }
    }

    public class NearestLocation
    {
        public string? __typename { get; set; }
        public string? path { get; set; }
        public string? fullName { get; set; }
        public string? id { get; set; }
    }

    public class Location
    {
        public string? __typename { get; set; }
        public string? fullName { get; set; }
        public string? path { get; set; }
    }

    public class Area
    {
        public string? __typename { get; set; }
        public string? units { get; set; }
        public int value { get; set; }
    }

    public class Price
    {
        public string? __typename { get; set; }
        public string? currency { get; set; }
        public int value { get; set; }
    }

    public class Company
    {
        public string? __typename { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
        public string? targetType { get; set; }
    }

    public class Category
    {
        public string? __typename { get; set; }
        public string? id { get; set; }
        public bool hasFloor { get; set; }
        public bool hasLandArea { get; set; }
        public string? name { get; set; }
        public string? pluralName { get; set; }
        public string? slug { get; set; }
        public string? title { get; set; }
    }

    public class City
    {
        public string? __typename { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
        public string? path { get; set; }
        public string? slug { get; set; }
    }

    public class Photo
    {
        public string? __typename { get; set; }
        public string? full { get; set; }
        public string? f660x496 { get; set; }
        public string? thumbnail { get; set; }
    }
}
