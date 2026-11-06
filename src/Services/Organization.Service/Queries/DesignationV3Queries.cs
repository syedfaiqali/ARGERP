using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed record GetDesignationsV3Query(
        int PageNumber,
        int PageSize,
        string? SearchTerm,
        int ClientId) : IRequest<PaginationResponse<DesignationV3Dto>>;

    public sealed record GetDesignationByIdV3Query(int Id, int ClientId) : IRequest<DesignationV3Dto?>;
}
