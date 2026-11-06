using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class AcademicYearV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("academicYear")]
        public string? AcademicYear { get; set; }

        [JsonPropertyName("academicRegisterId")]
        public int? AcademicRegisterId { get; set; }

        [JsonPropertyName("academicRegister")]
        public string? AcademicRegisterName { get; set; }

        [JsonPropertyName("semLovId")]
        public int? SemLovId { get; set; }
        [JsonPropertyName("courseCatId")]
        public int? CourseCatId { get; set; }

        [JsonPropertyName("semLovName")]
        public string? SemLovName { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("sessionNotVisible")]
        public bool SessionNotVisible { get; set; }

        [JsonPropertyName("currentYear")]
        public bool CurrentYear { get; set; }

        [JsonPropertyName("periods")]
        public List<AcademicYearPeriodV3Dto> Periods { get; set; } = new();
    }

    public sealed class AcademicYearPeriodV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("lineindex")]
        public int? LineIndex { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("monthid")]
        public int? MonthId { get; set; }

        [JsonPropertyName("periodstxt")]
        public string? PeriodName { get; set; }

        [JsonPropertyName("startdate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("enddate")]
        public DateTime? EndDate { get; set; }
    }

    public sealed class CreateAcademicYearV3Request
    {
        [JsonPropertyName("academicYear")]
        public string? AcademicYear { get; set; }

        [JsonPropertyName("academicRegisterId")]
        public int? AcademicRegisterId { get; set; }

        [JsonPropertyName("semLovId")]
        public int? SemLovId { get; set; }
        [JsonPropertyName("courseCatId")]
        public int? CourseCatId { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("sessionNotVisible")]
        public bool SessionNotVisible { get; set; }

        [JsonPropertyName("currentYear")]
        public bool CurrentYear { get; set; }

        [JsonPropertyName("periods")]
        public List<AcademicYearPeriodV3Dto> Periods { get; set; } = new();
    }

    public sealed class UpdateAcademicYearV3Request
    {
        [JsonPropertyName("academicYear")]
        public string? AcademicYear { get; set; }

        [JsonPropertyName("academicRegisterId")]
        public int? AcademicRegisterId { get; set; }

        [JsonPropertyName("semLovId")]
        public int? SemLovId { get; set; }
        [JsonPropertyName("courseCatId")]
        public int? CourseCatId { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("sessionNotVisible")]
        public bool SessionNotVisible { get; set; }

        [JsonPropertyName("currentYear")]
        public bool CurrentYear { get; set; }

        [JsonPropertyName("periods")]
        public List<AcademicYearPeriodV3Dto> Periods { get; set; } = new();
    }
}
