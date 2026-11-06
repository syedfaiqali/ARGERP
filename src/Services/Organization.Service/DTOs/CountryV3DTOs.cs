using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public class CountryV3Dto
    {
        [JsonPropertyName("currencycode")]
        public string CurrencyCode { get; set; } = "";
        [JsonPropertyName("currencyname")]
        public string CurrencyName { get; set; } = "";
        [JsonPropertyName("currencysymbol")]
        public string CurrencySymbol { get; set; } = "";
        [JsonPropertyName("countryid")]
        public int CountryId { get; set; }

        [JsonPropertyName("countrycode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("countryname")]
        public string CountryName { get; set; }

        [JsonPropertyName("isocode")]
        public string IsoCode { get; set; }

        [JsonPropertyName("dialcode")]
        public string DialCode { get; set; }
    }

    public class CreateCountryV3Request
    {
        [JsonPropertyName("countrycode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("countryname")]
        public string CountryName { get; set; }

        [JsonPropertyName("isocode")]
        public string IsoCode { get; set; }

        [JsonPropertyName("dialcode")]
        public string DialCode { get; set; }
    }

    public class UpdateCountryV3Request
    {
        [JsonPropertyName("countrycode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("countryname")]
        public string CountryName { get; set; }

        [JsonPropertyName("isocode")]
        public string IsoCode { get; set; }

        [JsonPropertyName("dialcode")]
        public string DialCode { get; set; }
    }
}
