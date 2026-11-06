using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed record CreateFOCalllogV3Command(CreateFOCalllogV3Request Request, int ClientId, int? EntityId) : IRequest<FOCalllogV3Dto>;
    public sealed record UpdateFOCalllogV3Command(int Id, UpdateFOCalllogV3Request Request, int ClientId, int? EntityId) : IRequest<FOCalllogV3Dto>;
    public sealed record DeleteFOCalllogV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
