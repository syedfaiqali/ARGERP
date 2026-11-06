using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateDeviceRegistrationV3Command(
        DeviceRegistrationUpsertV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<DeviceRegistrationV3Dto>;

    public sealed record UpdateDeviceRegistrationV3Command(
        int Id,
        DeviceRegistrationUpsertV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<DeviceRegistrationV3Dto?>;

    public sealed record DeleteDeviceRegistrationV3Command(
        int Id,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;
}
