using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class SiteV3Dto
    {
        [JsonPropertyName("siteid")]
        public int SiteId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string SiteName { get; set; } = string.Empty;

        [JsonPropertyName("sitetypeid")]
        public int SiteTypeId { get; set; }

        [JsonPropertyName("sitetypename")]
        public string SiteTypeName { get; set; } = string.Empty;

        [JsonPropertyName("siteforid")]
        public int SiteForId { get; set; }

        [JsonPropertyName("siteforname")]
        public string SiteForName { get; set; } = string.Empty;

        [JsonPropertyName("regionid")]
        public int RegionId { get; set; }

        [JsonPropertyName("regionname")]
        public string RegionName { get; set; } = string.Empty;

        [JsonPropertyName("forid")]
        public int? ForId { get; set; }

        [JsonPropertyName("forname")]
        public string? ForName { get; set; }
    }

    public sealed class SiteLookupDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("shortname")]
        public string ShortName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public sealed class CreateSiteV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string SiteName { get; set; } = string.Empty;

        [JsonPropertyName("sitetypeid")]
        public int SiteTypeId { get; set; }

        [JsonPropertyName("siteforid")]
        public int SiteForId { get; set; }

        [JsonPropertyName("regionid")]
        public int RegionId { get; set; }

        [JsonPropertyName("forid")]
        public int? ForId { get; set; }
    }

    public sealed class UpdateSiteV3Request
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string SiteName { get; set; } = string.Empty;

        [JsonPropertyName("sitetypeid")]
        public int SiteTypeId { get; set; }

        [JsonPropertyName("siteforid")]
        public int SiteForId { get; set; }

        [JsonPropertyName("regionid")]
        public int RegionId { get; set; }

        [JsonPropertyName("forid")]
        public int? ForId { get; set; }
    }
}
