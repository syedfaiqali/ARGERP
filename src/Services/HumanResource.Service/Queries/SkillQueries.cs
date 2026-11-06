using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Queries
{
    public sealed record GetSkillsV3Query(int PageNumber, int PageSize, int ClientId, int? EntityId, string? SearchTerm) : IRequest<PaginationResponse<SkillV3Dto>>;
    public sealed record GetSkillByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<SkillV3Dto?>;
    public sealed record GetSkillRatingsV3LookupQuery(int ClientId) : IRequest<IEnumerable<SkillRatingLookupDto>>;

    public sealed record SkillRatingLookupDto(int Id, string Code, string Name);
}
