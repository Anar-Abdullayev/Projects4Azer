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

        [JsonPropertyName("leased")]
        public bool? Leased { get; set; }

        [JsonPropertyName("floor")]
        public int? Floor { get; set; }

        [JsonPropertyName("floors")]
        public int? Floors { get; set; }

        [JsonPropertyName("hasMortgage")]
        public bool? HasMortgage { get; set; }

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

    }
}
