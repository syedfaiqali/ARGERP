using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Queries
{
    public sealed record GetEmployeeAttendanceV3Query(
        int ClientId,
        int? EntityId,
        int? EmployeeId,
        DateTime? FromDate,
        DateTime? TillDate) : IRequest<IReadOnlyList<EmployeeAttendanceV3Dto>>;

    public sealed record GetEmployeeAttendanceByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<EmployeeAttendanceV3Dto?>;

    public sealed record GetEmployeeAttendanceEmployeesV3Query(int ClientId, int? EntityId) : IRequest<IReadOnlyList<EmployeeAttendanceEmployeeDto>>;

    public sealed record GetEmployeeAttendanceActivitiesV3Query(int ClientId) : IRequest<IReadOnlyList<EmployeeAttendanceActivityDto>>;
}
