using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetAcademicRegistersV3Query : IRequest<PaginationResponse<AcademicRegisterV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int ClientId { get; set; }
        public int? EntityId { get; set; }
    }

    public sealed class GetAcademicRegisterByIdV3Query : IRequest<AcademicRegisterV3Dto>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
    }
}
