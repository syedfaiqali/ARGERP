using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class DesignationV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("designationName")]
        public string? DesignationName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("inactive")]
        public bool Inactive { get; set; }
    }

    public sealed class CreateDesignationV3Request
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("designationName")]
        public string? DesignationName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("inactive")]
        public bool Inactive { get; set; }
    }

    public sealed class UpdateDesignationV3Request
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("designationName")]
        public string? DesignationName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("inactive")]
        public bool Inactive { get; set; }
    }
}
