using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

namespace UniversalDataCatcher.Server.Bots.Bina.Models
{
    public class BinaAzResponseRoot
    {
        [JsonPropertyName("data")]
        public BinaAzData Data { get; set; }

    }

    public class BinaAzData
    {
        [JsonPropertyName("itemsConnection")]
        public ItemsConnection ItemsConnection { get; set; }
    }

    public class ItemsConnection
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("pageInfo")]
        public PageInfo PageInfo { get; set; }

        [JsonPropertyName("edges")]
        public List<Edge> Edges { get; set; }
    }

    public class PageInfo
    {
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }

        [JsonPropertyName("endCursor")]
        public string EndCursor { get; set; }
    }

    public class Edge
    {
        [JsonPropertyName("node")]
        public ESItem Node { get; set; }
    }

    public class ESItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("area")]
        public ESArea? Area { get; set; }

        [JsonPropertyName("leased")]
        public bool? Leased { get; set; }

        [JsonPropertyName("floor")]
        public int? Floor { get; set; }

        [JsonPropertyName("floors")]
        public int? Floors { get; set; }

        [JsonPropertyName("city")]
        public City? City { get; set; }

        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        [JsonPropertyName("hasMortgage")]
        public bool? HasMortgage { get; set; }

        [JsonPropertyName("price")]
        public ESPrice? Price { get; set; }

        [JsonPropertyName("company")]
        public Company? Company { get; set; }

        [JsonPropertyName("paidDaily")]
        public bool? PaidDaily { get; set; }

        [JsonPropertyName("rooms")]
        public int? Rooms { get; set; }

        [JsonPropertyName("hasBillOfSale")]
        public bool? HasBillOfSale { get; set; }

        [JsonPropertyName("hasRepair")]
        public bool? HasRepair { get; set; }

        [JsonPropertyName("vipped")]
        public bool? Vipped { get; set; }

        [JsonPropertyName("featured")]
        public bool? Featured { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("photosCount")]
        public int? PhotosCount { get; set; }

        [JsonPropertyName("isBusiness")]
        public bool? IsBusiness { get; set; }

        [JsonPropertyName("photos")]
        public List<ESPhoto>? Photos { get; set; }

        public BinaAzProperty GetInitialProperty()
        {
            BinaAzProperty property = new BinaAzProperty();
            property.City = City?.Name;
            property.Address = Location?.FullName;
            property.AdvLink = Constants.Constants.BaseUrl + Path;
            property.Id = int.Parse(Id!);
            property.Area = Area?.Value.ToString() + Area?.Units;
            property.Floor = Floor?.ToString() + "/" + Floors?.ToString();
            property.Price = Price?.Value.ToString();
            property.Currency = Price?.Currency;
            property.RoomCount = Rooms?.ToString();
            property.RentLong = Leased == true ? PaidDaily == true ? "Gündəlik" : "Aylıq" : "Satış";
            property.UpdatedTime = UpdatedAt;

            return property;
        }
    }

    public class ESArea
    {
        [JsonPropertyName("value")]
        public double? Value { get; set; }

        [JsonPropertyName("units")]
        public string? Units { get; set; }
    }

    public class ESPrice
    {
        [JsonPropertyName("value")]
        public decimal? Value { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    public class Company
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("targetType")]
        public string? TargetType { get; set; }
    }

    public class City
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }
    }

    public class ESPhoto
    {
        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; set; }

        [JsonPropertyName("f460x345")]
        public string? F460x345 { get; set; }

        [JsonPropertyName("large")]
        public string? Large { get; set; }
    }
}
