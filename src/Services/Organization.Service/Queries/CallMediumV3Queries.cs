using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public record GetCallMediumsV3Query(int PageNumber, int PageSize, string? SearchTerm) : IRequest<PaginationResponse<CallMediumV3Dto>>;
    public record GetCallMediumByIdV3Query(int Id) : IRequest<CallMediumV3Dto?>;
}
