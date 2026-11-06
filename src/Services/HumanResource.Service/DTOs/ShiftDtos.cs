using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public sealed record ShiftV3Dto(
        int Id,
        string Code,
        string Stxt,
        int? ShiftTypeId,
        string? ShiftTypeName,
        int? LovSharedId,
        string? LovSharedName,
        bool IsActive
    );

    public sealed record ShiftBreakV3Dto(
        int? Id,
        int LineIndex,
        string? Stxt,
        DateTime? StartTime,
        DateTime? EndTime,
        double? Duration
    );

    public sealed record ShiftDetailV3Dto(
        int? Id,
        int LineIndex,
        int DayId,
        DateTime? InTime,
        DateTime? OutTime,
        DateTime? StartDate,
        DateTime? EndDate,
        int? GraceTime,
        int? GraceTimeEnd,
        bool LatePolicyApplicable,
        bool EarlyPolicyApplicable,
        double? Duration,
        List<ShiftBreakV3Dto> Breaks
    );

    public sealed record ShiftSharedWithDetailV3Dto(
        int? Id,
        int LineIndex,
        int? LovSharedId,
        int? EntityId,
        int? DepartmentId,
        int? EmployeeId
    );

    public sealed record ShiftHeaderV3Dto(
        int Id,
        string Code,
        string Stxt,
        int? ShiftTypeId,
        string? ShiftTypeName,
        int? LovSharedId,
        string? LovSharedName,
        bool WoMonday,
        bool WoTuesday,
        bool WoWednesday,
        bool WoThuresday,
        bool WoFriday,
        bool WoSaturday,
        bool WoSunday,
        int? LateArrivPolicyId,
        int? EarlyDepPolicyId,
        int? OtPolicyId,
        int? AbsPolicyId,
        int? LateArrivPolicyMnt,
        int? EarlyDepPolicyMnt,
        int? OtPolicyMnt,
        bool IsActive,
        List<ShiftDetailV3Dto> ShiftDetails,
        List<ShiftSharedWithDetailV3Dto> SharedDetails
    );

    public sealed record CreateShiftV3Request(
        string? Code,
        string Stxt,
        int? ShiftTypeId,
        int? LovSharedId,
        bool WoMonday,
        bool WoTuesday,
        bool WoWednesday,
        bool WoThuresday,
        bool WoFriday,
        bool WoSaturday,
        bool WoSunday,
        int? LateArrivPolicyId,
        int? EarlyDepPolicyId,
        int? OtPolicyId,
        int? AbsPolicyId,
        int? LateArrivPolicyMnt,
        int? EarlyDepPolicyMnt,
        int? OtPolicyMnt,
        bool IsActive,
        List<ShiftDetailV3Dto> ShiftDetails,
        List<ShiftSharedWithDetailV3Dto> SharedDetails
    );

    public sealed record UpdateShiftV3Request(
        int Id,
        string? Code,
        string Stxt,
        int? ShiftTypeId,
        int? LovSharedId,
        bool WoMonday,
        bool WoTuesday,
        bool WoWednesday,
        bool WoThuresday,
        bool WoFriday,
        bool WoSaturday,
        bool WoSunday,
        int? LateArrivPolicyId,
        int? EarlyDepPolicyId,
        int? OtPolicyId,
        int? AbsPolicyId,
        int? LateArrivPolicyMnt,
        int? EarlyDepPolicyMnt,
        int? OtPolicyMnt,
        bool IsActive,
        List<ShiftDetailV3Dto> ShiftDetails,
        List<ShiftSharedWithDetailV3Dto> SharedDetails
    );
}
