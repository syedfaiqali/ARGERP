using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public sealed record CustomShiftV3Dto(
        int Id,
        string Code,
        string Stxt,
        int? ShiftTypeId,
        string? ShiftTypeName,
        DateTime? StartTime,
        DateTime? EndTime,
        int? GraceTime,
        int? GraceTimeEd,
        int? EntityId,
        string? EntityName,
        bool IsActive
    );


    public sealed record CustomShiftBreakV3Dto(
        int? Id,
        int LineIndex,
        string? Stxt,
        DateTime? StartTime,
        DateTime? EndTime,
        double? Duration
    );

    public sealed record CustomShiftHeaderV3Dto(
        int Id,
        string Code,
        string Stxt,
        int? ShiftTypeId,
        string? ShiftTypeName,
        DateTime? StartTime,
        DateTime? EndTime,
        int? GraceTime,
        int? GraceTimeEd,
        bool WoMonday,
        bool WoTuesday,
        bool WoWednesday,
        bool WoThuresday,
        bool WoFriday,
        bool WoSaturday,
        bool WoSunday,
        int? EntityId,
        string? EntityName,
        bool IsActive,
        List<CustomShiftBreakV3Dto> Breaks
    );

    public sealed record CreateCustomShiftV3Request(
        string? Code,
        string Stxt,
        int? ShiftTypeId,
        DateTime? StartTime,
        DateTime? EndTime,
        int? GraceTime,
        int? GraceTimeEd,
        bool WoMonday,
        bool WoTuesday,
        bool WoWednesday,
        bool WoThuresday,
        bool WoFriday,
        bool WoSaturday,
        bool WoSunday,
        int? EntityId,
        bool IsActive,
        List<CustomShiftBreakV3Dto> Breaks
    );

    public sealed record UpdateCustomShiftV3Request(
        int Id,
        string? Code,
        string Stxt,
        int? ShiftTypeId,
        DateTime? StartTime,
        DateTime? EndTime,
        int? GraceTime,
        int? GraceTimeEd,
        bool WoMonday,
        bool WoTuesday,
        bool WoWednesday,
        bool WoThuresday,
        bool WoFriday,
        bool WoSaturday,
        bool WoSunday,
        int? EntityId,
        bool IsActive,
        List<CustomShiftBreakV3Dto> Breaks
    );
}
