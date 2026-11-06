using HumanResource.Service.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HumanResource.Service.Commands
{
    public record BulkSaveEmployeeDailyAttendanceV3Command(
        BulkSaveEmployeeDailyAttendanceV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<IReadOnlyList<EmployeeDailyAttendanceV3Dto>>;

    public record CreateEmployeeDailyAttendanceV3Command(
        CreateEmployeeDailyAttendanceV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<EmployeeDailyAttendanceV3Dto>;

    public record UpdateEmployeeDailyAttendanceV3Command(
        UpdateEmployeeDailyAttendanceV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<EmployeeDailyAttendanceV3Dto>;

    public record DeleteEmployeeDailyAttendanceV3Command(
        int Id,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;
}
