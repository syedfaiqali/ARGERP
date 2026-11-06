using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetAcademicYearsV3Query : IRequest<PaginationResponse<AcademicYearV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int ClientId { get; set; }
        public int? EntityId { get; set; }
        public bool ActiveOnly { get; set; }
    }

    public sealed class GetAcademicYearByIdV3Query : IRequest<AcademicYearV3Dto>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
    }

    public sealed class GetAcademicYearSemesterLovsV3Query : IRequest<List<LookupItemDto>>
    {
    }

    public sealed record GetAcademicRegisterStartDateV3Query(int AcademicRegisterId, int ClientId, int? AcademicYearId) : IRequest<DateTime?>;

    public sealed class GetGeneratedPeriodsV3Query : IRequest<List<AcademicYearPeriodV3Dto>>
    {
        public int ClientId { get; set; }
        public int? AcademicRegisterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
