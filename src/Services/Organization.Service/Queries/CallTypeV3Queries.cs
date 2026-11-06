using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public record GetCallTypesV3Query(int PageNumber, int PageSize, string? SearchTerm) : IRequest<PaginationResponse<CallTypeV3Dto>>;
    public record GetCallTypeByIdV3Query(int Id) : IRequest<CallTypeV3Dto?>;
}
