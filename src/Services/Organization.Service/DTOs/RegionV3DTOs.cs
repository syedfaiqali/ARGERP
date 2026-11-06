using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class RegionV3Dto
    {
        [JsonPropertyName("regionid")]
        public int RegionId { get; set; }

        [JsonPropertyName("regioncode")]
        public string RegionCode { get; set; }

        [JsonPropertyName("regionname")]
        public string RegionName { get; set; }

        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("countryname")]
        public string CountryName { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateRegionV3Request
    {
        [JsonPropertyName("regioncode")]
        public string RegionCode { get; set; }

        [JsonPropertyName("regionname")]
        public string RegionName { get; set; }

        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateRegionV3Request
    {
        [JsonPropertyName("regioncode")]
        public string RegionCode { get; set; }

        [JsonPropertyName("regionname")]
        public string RegionName { get; set; }

        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }
    }
}
