using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateAcademicYearV3Command : IRequest<AcademicYearV3Dto>
    {
        public string? AcademicYear { get; set; }
        public int? AcademicRegisterId { get; set; }
        public int? SemLovId { get; set; }
        public int? CourseCatId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public bool SessionNotVisible { get; set; }
        public bool CurrentYear { get; set; }
        public List<AcademicYearPeriodV3Dto> Periods { get; set; } = new();
        public int ClientId { get; set; }
        public int? EntityId { get; set; }
        public int CreatedBy { get; set; }
    }

    public sealed class UpdateAcademicYearV3Command : IRequest<AcademicYearV3Dto>
    {
        public int Id { get; set; }
        public string? AcademicYear { get; set; }
        public int? AcademicRegisterId { get; set; }
        public int? SemLovId { get; set; }
        public int? CourseCatId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public bool SessionNotVisible { get; set; }
        public bool CurrentYear { get; set; }
        public List<AcademicYearPeriodV3Dto> Periods { get; set; } = new();
        public int ClientId { get; set; }
        public int? EntityId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public sealed class DeleteAcademicYearV3Command : IRequest<bool>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int DeletedBy { get; set; }
    }
}
