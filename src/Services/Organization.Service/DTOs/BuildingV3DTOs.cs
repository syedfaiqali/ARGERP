using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class BuildingV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string BuildingName { get; set; } = string.Empty;

        [JsonPropertyName("entityid")]
        public int? EntityId { get; set; }

        [JsonPropertyName("clientid")]
        public int? ClientId { get; set; }
    }

    public sealed class CreateBuildingV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string BuildingName { get; set; } = string.Empty;
    }

    public sealed class UpdateBuildingV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string BuildingName { get; set; } = string.Empty;
    }
}
