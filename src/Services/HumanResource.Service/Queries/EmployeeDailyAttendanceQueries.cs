using HumanResource.Service.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace HumanResource.Service.Queries
{
    public record GetEmployeeDailyAttendanceV3Query(
        int ClientId,
        int? EntityId,
        int? EmployeeId,
        DateTime? FromDate,
        DateTime? TillDate,
        int? DepartmentId = null,
        int? DesignationId = null
    ) : IRequest<IReadOnlyList<EmployeeDailyAttendanceV3Dto>>;

    public record GetEmployeeDailyAttendanceByIdV3Query(
        long Id,
        int ClientId,
        int? EntityId
    ) : IRequest<EmployeeDailyAttendanceV3Dto?>;
}
