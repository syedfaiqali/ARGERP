using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetSkillTypesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm
    ) : IRequest<PaginationResponse<SkillTypeV3Dto>>;

    public sealed record GetSkillTypeByIdV3Query(
        int Id,
        int ClientId,
        int? EntityId
    ) : IRequest<SkillTypeV3Dto?>;
}
