using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetDeviceRegistrationLookupsV3Query(
        int ClientId,
        int? EntityId
    ) : IRequest<DeviceRegistrationLookupsV3Dto>;

    public sealed record GetDeviceRegistrationsV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm = null
    ) : IRequest<PaginationResponse<DeviceRegistrationV3Dto>>;

    public sealed record GetDeviceRegistrationByIdV3Query(
        int Id,
        int ClientId,
        int? EntityId
    ) : IRequest<DeviceRegistrationV3Dto?>;
}
