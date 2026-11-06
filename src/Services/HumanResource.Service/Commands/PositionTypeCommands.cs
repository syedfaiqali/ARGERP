using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Commands
{
    public sealed record CreatePositionTypeV3Command(CreatePositionTypeV3Request Request, int ClientId, int? EntityId) : IRequest<PositionTypeV3Dto>;
    public sealed record UpdatePositionTypeV3Command(UpdatePositionTypeV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeletePositionTypeV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
