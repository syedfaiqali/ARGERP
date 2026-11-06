using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Queries
{
    public sealed record GetQualificationTypesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm = null) : IRequest<PaginationResponse<QualificationTypeV3Dto>>;

    public sealed record GetQualificationTypeByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<QualificationTypeV3Dto?>;
}
