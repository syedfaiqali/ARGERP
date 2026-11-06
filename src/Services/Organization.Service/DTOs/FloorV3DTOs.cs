using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class FloorV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string FloorName { get; set; } = string.Empty;

        [JsonPropertyName("buildingid")]
        public int BuildingId { get; set; }

        [JsonPropertyName("buildingname")]
        public string BuildingName { get; set; } = string.Empty;

        [JsonPropertyName("entityid")]
        public int? EntityId { get; set; }

        [JsonPropertyName("clientid")]
        public int? ClientId { get; set; }
    }

    public sealed class CreateFloorV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string FloorName { get; set; } = string.Empty;

        [JsonPropertyName("buildingid")]
        public int BuildingId { get; set; }
    }

    public sealed class UpdateFloorV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string FloorName { get; set; } = string.Empty;

        [JsonPropertyName("buildingid")]
        public int BuildingId { get; set; }
    }
}
