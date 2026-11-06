using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class DonorV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("donorName")]
        public string? DonorName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("contactNo")]
        public string? ContactNo { get; set; }
    }

    public sealed class CreateDonorV3Request
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("donorName")]
        public string? DonorName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("contactNo")]
        public string? ContactNo { get; set; }
    }

    public sealed class UpdateDonorV3Request
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("donorName")]
        public string? DonorName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("contactNo")]
        public string? ContactNo { get; set; }
    }
}
