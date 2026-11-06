namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeAttendanceV3Dto(
        int Id,
        int? EmpId,
        string EmployeeCode,
        string EmployeeName,
        string Department,
        string Designation,
        int? ActivityId,
        string Activity,
        DateTime? AttendanceDate,
        DateTime? AttendanceTime,
        string Remarks,
        string MachineCode,
        int? ShiftId,
        string Shift
    );

    public sealed record EmployeeAttendanceEmployeeDto(
        int Id,
        string EmployeeCode,
        string EmployeeName,
        string Department,
        string Designation,
        string MachineCode,
        int? ShiftId,
        string Shift,
        int? EmployeeTypeId,
        string EmployeeType
    );

    public sealed record EmployeeAttendanceActivityDto(
        int Id,
        string Activity
    );

    public sealed record CreateEmployeeAttendanceV3Request(
        int EmpId,
        int AtnActivityId,
        DateTime AttendanceDate,
        DateTime AttendanceTime,
        string? Remarks
    );

    public sealed record UpdateEmployeeAttendanceV3Request(
        int Id,
        int EmpId,
        int AtnActivityId,
        DateTime AttendanceDate,
        DateTime AttendanceTime,
        string? Remarks
    );
}
