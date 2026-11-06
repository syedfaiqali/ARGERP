using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class AcademicRegisterV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("register")]
        public string RegisterName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("clientid")]
        public int? ClientId { get; set; }
    }

    public sealed class CreateAcademicRegisterV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("register")]
        public string RegisterName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public sealed class UpdateAcademicRegisterV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("register")]
        public string RegisterName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
