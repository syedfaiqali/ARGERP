using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public sealed record ScheduleV3Dto(
        int Id,
        string Code,
        string Stxt,
        DateTime? FromDate,
        DateTime? TillDate,
        int? ScheduleForId,
        string? ScheduleForName,
        int? ShiftId,
        string? ShiftName,
        int? AssignToId
    );

    public sealed record ScheduleDetailV3Dto(
        int? Id,
        int LineIndex,
        DateTime? SchDate,
        bool Holiday,
        int? ShiftId,
        DateTime? TimeIn,
        DateTime? TimeOut,
        double? Duration,
        string? DayName
    );

    public sealed record ScheduleHeaderV3Dto(
        int Id,
        string Code,
        string Stxt,
        DateTime? FromDate,
        DateTime? TillDate,
        int? ScheduleForId,
        string? ScheduleForName,
        int? ShiftId,
        string? ShiftName,
        int? AssignToId,
        List<ScheduleDetailV3Dto> Details
    );

    public sealed record CreateScheduleV3Request(
        string? Code,
        string Stxt,
        DateTime? FromDate,
        DateTime? TillDate,
        int? ScheduleForId,
        int? ShiftId,
        int? AssignToId,
        List<ScheduleDetailV3Dto> Details
    );

    public sealed record UpdateScheduleV3Request(
        int Id,
        string? Code,
        string Stxt,
        DateTime? FromDate,
        DateTime? TillDate,
        int? ScheduleForId,
        int? ShiftId,
        int? AssignToId,
        List<ScheduleDetailV3Dto> Details
    );
}
