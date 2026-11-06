using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetDeviceTypesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        string? SearchTerm
    ) : IRequest<PaginationResponse<DeviceTypeV3Dto>>;

    public sealed record GetDeviceTypeByIdV3Query(
        int Id,
        int ClientId
    ) : IRequest<DeviceTypeV3Dto?>;
}
