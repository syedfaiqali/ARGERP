using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed record GetOccupationsV3Query(
        int PageNumber,
        int PageSize,
        string? SearchTerm,
        int ClientId) : IRequest<PaginationResponse<OccupationV3Dto>>;

    public sealed record GetOccupationByIdV3Query(int Id, int ClientId) : IRequest<OccupationV3Dto?>;
}
