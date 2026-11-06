using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public record GetAuthoritiesV3Query(
        int ClientId,
        int? EntityId,
        int PageNumber = 1,
        int PageSize = 10,
        string? Search = null) : IRequest<PaginationResponse<AuthorityV3Dto>>;

    public record GetAuthorityByIdV3Query(
        int ClientId,
        int? EntityId,
        int Id) : IRequest<AuthorityV3Dto?>;
}
