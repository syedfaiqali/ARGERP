namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeInformationRecordV3Dto(
        int Id,
        int EmployeeId,
        string Group,
        int CategoryId,
        string Category,
        string Title,
        string? ReferenceNumber,
        DateTime RecordDate,
        DateTime? EndDate,
        string? Status,
        string? Authority,
        string? SubCategory,
        string? Mode,
        string? Details,
        string? Outcome,
        string? Remarks,
        bool HasAttachment,
        string? FileName,
        DateTime CreatedAt,
        string? CreatedBy,
        DateTime? UpdatedAt,
        string? UpdatedBy
    );

    public sealed class UpsertEmployeeInformationRecordV3Request
    {
        public int EmployeeId { get; set; }
        public int CategoryId { get; set; }
        public string? Title { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? RecordDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? Authority { get; set; }
        public string? SubCategory { get; set; }
        public string? Mode { get; set; }
        public string? Details { get; set; }
        public string? Outcome { get; set; }
        public string? Remarks { get; set; }
        public string? FileName { get; set; }
        public string? FileContentBase64 { get; set; }
        public bool RemoveAttachment { get; set; }
    }
}
