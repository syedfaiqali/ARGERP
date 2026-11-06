using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public record EmployeeAttendanceDashboardMarkDto(
        long Id,
        int EmployeeId,
        DateTime AtnDate,
        bool IsAbsent,
        bool IsHoliday,
        int LateMin,
        DateTime? ActShiftIn,
        DateTime? ActShiftOut
    );

    public record EmployeeDailyAttendanceV3Dto(
        long Id,
        int? EmployeeId,
        string EmployeeCode,
        string EmployeeName,
        DateTime? AtnDate,
        string Day,
        DateTime? SchTimeIn,
        DateTime? SchTimeOut,
        DateTime? ActShiftIn,
        DateTime? ActShiftOut,
        int? LateMin,
        int? LateCount,
        int? LateHalfDays,
        int? LateAbsent,
        int? LateExcuse,
        int? EdMin,
        int? EdCount,
        int? EdHalfDays,
        int? EdAbsent,
        int? EdExcuse,
        int? OtMin,
        int? OtCount,
        int? OtHalfDays,
        string Remarks,
        bool IsAbsent,
        bool IsHoliday,
        int? ShiftId,
        int? ShiftDtlId,
        int? DayId,
        int? DepartmentId,
        int? DesignationId
    );

    public record CreateEmployeeDailyAttendanceV3Request(
        int EmployeeId,
        DateTime AtnDate,
        DateTime? SchTimeIn,
        DateTime? SchTimeOut,
        DateTime? ActShiftIn,
        DateTime? ActShiftOut,
        int? LateMin,
        int? LateCount,
        int? LateHalfDays,
        int? LateAbsent,
        int? LateExcuse,
        int? EdMin,
        int? EdCount,
        int? EdHalfDays,
        int? EdAbsent,
        int? EdExcuse,
        int? OtMin,
        int? OtCount,
        int? OtHalfDays,
        string Remarks,
        bool IsAbsent,
        bool IsHoliday,
        int? ShiftId = null,
        int? ShiftDtlId = null,
        int? DayId = null,
        int? DepartmentId = null,
        int? DesignationId = null
    );

    public record UpdateEmployeeDailyAttendanceV3Request(
        int Id,
        int EmployeeId,
        DateTime AtnDate,
        DateTime? SchTimeIn,
        DateTime? SchTimeOut,
        DateTime? ActShiftIn,
        DateTime? ActShiftOut,
        int? LateMin,
        int? LateCount,
        int? LateHalfDays,
        int? LateAbsent,
        int? LateExcuse,
        int? EdMin,
        int? EdCount,
        int? EdHalfDays,
        int? EdAbsent,
        int? EdExcuse,
        int? OtMin,
        int? OtCount,
        int? OtHalfDays,
        string Remarks,
        bool IsAbsent,
        bool IsHoliday,
        int? ShiftId = null,
        int? ShiftDtlId = null,
        int? DayId = null,
        int? DepartmentId = null,
        int? DesignationId = null
    );

    public record BulkSaveEmployeeDailyAttendanceV3Request(
        IReadOnlyList<CreateEmployeeDailyAttendanceV3Request> Items
    );

    public record DailyAttendanceListV3(
        IReadOnlyList<EmployeeDailyAttendanceV3Dto> Items,
        int TotalCount
    );
}
