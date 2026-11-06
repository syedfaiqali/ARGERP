using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class OccupationV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("occupationName")]
        public string? OccupationName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public sealed class CreateOccupationV3Request
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("occupationName")]
        public string? OccupationName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public sealed class UpdateOccupationV3Request
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("occupationName")]
        public string? OccupationName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
