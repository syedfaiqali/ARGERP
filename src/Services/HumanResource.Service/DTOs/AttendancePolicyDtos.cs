using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public sealed record AttendancePolicyV3Dto(
        int Id,
        string Code,
        string Stxt,
        int? PolicyTypeId,
        string? PolicyTypeName,
        bool ApplyOffDays,
        int NoOfExcuse,
        bool IsActive
    );

    public sealed record AttendancePolicyDetailV3Dto(
        int? Id,
        int LineIndex,
        int? StartTime,
        int? EndTime,
        int? PeriodTypeId,
        string? PeriodTypeName,
        int? ValTypeId,
        string? ValTypeName,
        double? Value
    );

    public sealed record AttendancePolicyAbsentDetailV3Dto(
        int? Id,
        int LineIndex,
        int? PeriodTypeId,
        string? PeriodTypeName,
        double? PolicyCount,
        double? AbsentCount
    );

    public sealed record AttendancePolicyHeaderV3Dto(
        int Id,
        string Code,
        string Stxt,
        int? PolicyTypeId,
        string? PolicyTypeName,
        bool ApplyOffDays,
        int NoOfExcuse,
        bool IsActive,
        List<AttendancePolicyDetailV3Dto> PolicyDetails,
        List<AttendancePolicyAbsentDetailV3Dto> AbsentDetails
    );

    public sealed record CreateAttendancePolicyV3Request(
        string? Code,
        string Stxt,
        int? PolicyTypeId,
        bool ApplyOffDays,
        int? NoOfExcuse,
        bool IsActive,
        List<AttendancePolicyDetailV3Dto> PolicyDetails,
        List<AttendancePolicyAbsentDetailV3Dto> AbsentDetails
    );

    public sealed record UpdateAttendancePolicyV3Request(
        int Id,
        string? Code,
        string Stxt,
        int? PolicyTypeId,
        bool ApplyOffDays,
        int? NoOfExcuse,
        bool IsActive,
        List<AttendancePolicyDetailV3Dto> PolicyDetails,
        List<AttendancePolicyAbsentDetailV3Dto> AbsentDetails
    );
}
