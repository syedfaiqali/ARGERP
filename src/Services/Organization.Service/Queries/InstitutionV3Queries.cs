using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed record GetInstitutionsV3Query(
        int PageNumber,
        int PageSize,
        string? SearchTerm,
        int ClientId,
        bool Lightweight = false) : IRequest<PaginationResponse<InstitutionV3Dto>>;

    public sealed record GetInstitutionByIdV3Query(
        int Id,
        int ClientId) : IRequest<InstitutionV3Dto?>;

    public sealed record GetInstitutionLookupsV3Query(
        int ClientId,
        int? CountryId,
        int? StateId) : IRequest<InstitutionLookupsV3Dto>;
}
