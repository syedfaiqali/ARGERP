using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateSkillTypeV3Command(
        CreateSkillTypeV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<SkillTypeV3Dto>;

    public sealed record UpdateSkillTypeV3Command(
        UpdateSkillTypeV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;

    public sealed record DeleteSkillTypeV3Command(
        int Id,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;
}
