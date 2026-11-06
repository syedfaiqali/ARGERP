using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetUploadAttendanceLookupsV3Query(
        int ClientId
    ) : IRequest<UploadAttendanceLookupsV3Dto>;

    public sealed record GetUploadAttendanceEmployeesV3Query(
        int ClientId,
        int? EntityId
    ) : IRequest<IReadOnlyList<UploadAttendanceEmployeeV3Dto>>;
}
