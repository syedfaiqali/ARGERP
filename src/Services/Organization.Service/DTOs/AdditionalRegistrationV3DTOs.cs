using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class AdditionalRegistrationV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("configid")]
        public int ConfigId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("stxt")]
        public string SectionName { get; set; } = string.Empty;

        [JsonPropertyName("caption")]
        public string Caption { get; set; } = string.Empty;

        [JsonPropertyName("infoforid")]
        public int InfoForId { get; set; }

        [JsonPropertyName("sequance")]
        public int? Sequance { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("route")]
        public string Route { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonPropertyName("_class")]
        public string CssClass { get; set; } = string.Empty;
    }

    public sealed class UpsertAdditionalRegistrationV3Request
    {
        [JsonPropertyName("configid")]
        public int ConfigId { get; set; }

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("infoforid")]
        public int InfoForId { get; set; }

        [JsonPropertyName("sequance")]
        public int? Sequance { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }
    }

    public sealed class AdditionalRegistrationInfoForOptionV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
