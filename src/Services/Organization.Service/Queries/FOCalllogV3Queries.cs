using MediatR;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure;

namespace Organization.Service.Queries
{
    public sealed record GetFOCalllogV3Query(int PageNumber, int PageSize, string? SearchTerm, int ClientId, int? EntityId, int? PurposeTypeId, int? CallTypeId, int? CallMediumId) : IRequest<PaginationResponse<FOCalllogV3Dto>>;
    public sealed record GetFOCalllogByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<FOCalllogV3Dto?>;
    public sealed record GetCallTypeV3Query() : IRequest<List<ComplainTypeLookupDto>>;
    public sealed record GetCallMediumV3Query() : IRequest<List<ComplainTypeLookupDto>>;
}
