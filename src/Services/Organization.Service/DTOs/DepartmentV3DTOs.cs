using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class DepartmentV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("departmentName")]
        public string? DepartmentName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("chartOfAccountId")]
        public int? ChartOfAccountId { get; set; }

        [JsonPropertyName("chartOfAccount")]
        public string? ChartOfAccount { get; set; }

        [JsonPropertyName("receivingChartOfAccountId")]
        public int? ReceivingChartOfAccountId { get; set; }

        [JsonPropertyName("receivingChartOfAccount")]
        public string? ReceivingChartOfAccount { get; set; }
    }

    public sealed class FicoaV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    public sealed class CreateDepartmentV3Request
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("departmentName")]
        public string? DepartmentName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("chartOfAccountId")]
        public int? ChartOfAccountId { get; set; }

        [JsonPropertyName("receivingChartOfAccountId")]
        public int? ReceivingChartOfAccountId { get; set; }
    }

    public sealed class UpdateDepartmentV3Request
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("departmentName")]
        public string? DepartmentName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("chartOfAccountId")]
        public int? ChartOfAccountId { get; set; }

        [JsonPropertyName("receivingChartOfAccountId")]
        public int? ReceivingChartOfAccountId { get; set; }
    }
}
