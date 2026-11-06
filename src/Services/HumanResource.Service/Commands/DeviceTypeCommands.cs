using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateDeviceTypeV3Command(
        CreateDeviceTypeV3Request Request,
        int ClientId
    ) : IRequest<DeviceTypeV3Dto>;

    public sealed record UpdateDeviceTypeV3Command(
        UpdateDeviceTypeV3Request Request,
        int ClientId
    ) : IRequest<bool>;

    public sealed record DeleteDeviceTypeV3Command(
        int Id,
        int ClientId
    ) : IRequest<bool>;
}
