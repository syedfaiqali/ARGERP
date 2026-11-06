using HumanResource.Service.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HumanResource.Service.Commands
{
    public sealed record SaveEmployeeMonthlyAttendanceV3Command(
        IReadOnlyList<EmployeeMonthlyAttendanceUpsertV3Request> Items,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;

    public sealed record RevertEmployeeMonthlyAttendanceV3Command(
        int EmployeeId,
        int RegisterId,
        int PeriodDetailId,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;
}
