using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Commands
{
    public sealed record CreateSkillV3Command(CreateSkillV3Request Request, int ClientId, int? EntityId) : IRequest<SkillV3Dto>;
    public sealed record UpdateSkillV3Command(UpdateSkillV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteSkillV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
