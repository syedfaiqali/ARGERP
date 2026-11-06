using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public sealed class EmployeeInformationRecord
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public int? EntityId { get; set; }

        // References lov.id (vtype = HRRCAT); never persist the display name/code here.
        public int CategoryId { get; set; }

        /// <summary>Legacy HREmployeeInformationRecord.category (NOT NULL): the HRRCAT lov code, kept in sync with CategoryId on every write.</summary>
        [MaxLength(30)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        public DateTime RecordDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(200)]
        public string? Authority { get; set; }

        [MaxLength(100)]
        public string? SubCategory { get; set; }

        [MaxLength(50)]
        public string? Mode { get; set; }

        [MaxLength(2000)]
        public string? Details { get; set; }

        [MaxLength(2000)]
        public string? Outcome { get; set; }

        [MaxLength(2000)]
        public string? Remarks { get; set; }

        [MaxLength(260)]
        public string? FileName { get; set; }

        [MaxLength(150)]
        public string? FileContentType { get; set; }

        public byte[]? FileData { get; set; }
        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
    }
}
