using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateEarningTypeV3Command(CreateEarningTypeV3Request Request, int ClientId, int? EntityId) : IRequest<EarningTypeV3Dto>;
    public sealed record UpdateEarningTypeV3Command(UpdateEarningTypeV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteEarningTypeV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
