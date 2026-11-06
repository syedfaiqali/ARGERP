using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class CityV3Dto
    {
        [JsonPropertyName("cityid")]
        public int CityId { get; set; }

        [JsonPropertyName("citycode")]
        public string CityCode { get; set; }

        [JsonPropertyName("cityname")]
        public string CityName { get; set; }

        [JsonPropertyName("dialcode")]
        public string DialCode { get; set; }

        [JsonPropertyName("stateid")]
        public int StateId { get; set; }

        [JsonPropertyName("statename")]
        public string StateName { get; set; }

        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("countryname")]
        public string CountryName { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateCityV3Request
    {
        [JsonPropertyName("citycode")]
        public string CityCode { get; set; }

        [JsonPropertyName("cityname")]
        public string CityName { get; set; }

        [JsonPropertyName("dialcode")]
        public string DialCode { get; set; }

        [JsonPropertyName("stateid")]
        public int StateId { get; set; }

        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateCityV3Request
    {
        [JsonPropertyName("citycode")]
        public string CityCode { get; set; }

        [JsonPropertyName("cityname")]
        public string CityName { get; set; }

        [JsonPropertyName("dialcode")]
        public string DialCode { get; set; }

        [JsonPropertyName("stateid")]
        public int StateId { get; set; }

        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("isactive")]
        public bool IsActive { get; set; }
    }
}
