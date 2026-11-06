using HumanResource.Service.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HumanResource.Service.Queries
{
    public sealed record GetEmployeeMonthlyAttendanceProcessV3Query(
        int ClientId,
        int? EntityId,
        int PeriodDetailId,
        int? SiteId,
        int? DepartmentId,
        int? DesignationId,
        int? EmployeeId
    ) : IRequest<IReadOnlyList<EmployeeMonthlyAttendanceV3Dto>>;

    public sealed record GetPayrollPeriodDetailsByRegisterV3Query(
        int ClientId,
        int? EntityId,
        int RegisterId
    ) : IRequest<IReadOnlyList<PayrollPeriodDetailLookupV3Dto>>;
}
