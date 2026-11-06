using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class StateV3Dto
    {
        [JsonPropertyName("stateid")]
        public int StateId { get; set; }

        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("statename")]
        public string StateName { get; set; }

        [JsonPropertyName("statecode")]
        public string StateCode { get; set; }

        [JsonPropertyName("countryname")]
        public string CountryName { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateStateV3Request
    {
        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("statename")]
        public string StateName { get; set; }

        [JsonPropertyName("statecode")]
        public string StateCode { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateStateV3Request
    {
        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("statename")]
        public string StateName { get; set; }

        [JsonPropertyName("statecode")]
        public string StateCode { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }
    }
}
