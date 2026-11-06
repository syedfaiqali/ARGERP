using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Commands
{
    public sealed record CreateShiftV3Command(CreateShiftV3Request Request, int ClientId, int? EntityId) : IRequest<ShiftHeaderV3Dto>;
    public sealed record UpdateShiftV3Command(UpdateShiftV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteShiftV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
