using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeMonthlyAttendanceV3Dto(
        int EmployeeId,
        string EmployeeCode,
        string EmployeeName,
        string Department,
        string Designation,
        double? Days,
        double? PresentsCount,
        double? AbsentsCount,
        double? ActualDays,
        double? ActualPresent,
        double? Holidays,
        int? Leaves,
        double? LateCount,
        double? EarlyCounts,
        double? HalfDaysCount,
        int? RegisterId,
        int? PayPeriodId,
        int? PayPeriodDetailId,
        DateTime? StartDate,
        DateTime? EndDate,
        DateTime? DisbDate,
        int? EntityId
    );

    public sealed record SaveEmployeeMonthlyAttendanceV3Request(
        IReadOnlyList<EmployeeMonthlyAttendanceUpsertV3Request> Items
    );

    public sealed record EmployeeMonthlyAttendanceUpsertV3Request(
        int EmployeeId,
        int? RegisterId,
        int? PayPeriodId,
        int? PayPeriodDetailId,
        DateTime? StartDate,
        DateTime? EndDate,
        DateTime? DisbDate,
        int? EntityId,
        double? Days,
        double? PresentsCount,
        double? AbsentsCount,
        double? HalfDaysCount,
        double? LateCount,
        double? EarlyCounts,
        double? ActualDays,
        double? ActualPresent,
        double? Holidays,
        int? Leaves
    );

    public sealed record PayrollPeriodDetailLookupV3Dto(
        int Id,
        int? PayPeriodId,
        string PeriodText,
        DateTime? StartDate,
        DateTime? EndDate,
        int? NoOfDays,
        DateTime? DisbDate
    );
}
