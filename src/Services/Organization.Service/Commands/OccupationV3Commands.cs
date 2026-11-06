using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed record CreateOccupationV3Command(
        CreateOccupationV3Request Request,
        int ClientId) : IRequest<OccupationV3Dto>;

    public sealed record UpdateOccupationV3Command(
        UpdateOccupationV3Request Request,
        int ClientId) : IRequest<bool>;

    public sealed record DeleteOccupationV3Command(int Id, int ClientId) : IRequest<bool>;
}
