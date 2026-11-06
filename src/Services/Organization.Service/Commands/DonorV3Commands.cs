using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed record CreateDonorV3Command(
        CreateDonorV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<DonorV3Dto>;

    public sealed record UpdateDonorV3Command(
        UpdateDonorV3Request Request,
        int ClientId) : IRequest<bool>;

    public sealed record DeleteDonorV3Command(int Id, int ClientId) : IRequest<bool>;
}
