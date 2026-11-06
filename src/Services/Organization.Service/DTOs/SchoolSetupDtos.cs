namespace Organization.Service.DTOs
{
    public sealed class SchoolSetupColumnDto
    {
        public string Field { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
    }

    public sealed class SchoolSetupTabResponseDto
    {
        public string Tab { get; set; } = string.Empty;
        public List<SchoolSetupColumnDto> Columns { get; set; } = new();
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public sealed class SchoolSetupProcessRequestDto
    {
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
    }

    public sealed class SchoolSetupProcessResponseDto
    {
        public string Tab { get; set; } = string.Empty;
        public int ProcessedCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
