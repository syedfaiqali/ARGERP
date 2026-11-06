using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateCustomShiftV3Command(CreateCustomShiftV3Request Request, int ClientId, int? EntityId) : IRequest<CustomShiftHeaderV3Dto>;
    public sealed record UpdateCustomShiftV3Command(UpdateCustomShiftV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteCustomShiftV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
