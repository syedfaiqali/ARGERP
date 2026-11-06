using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Queries
{
    public sealed record GetQualificationsV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        int? LovtypeId = null,
        string? SearchTerm = null) : IRequest<PaginationResponse<QualificationV3Dto>>;

    public sealed record GetQualificationByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<QualificationV3Dto?>;
}
