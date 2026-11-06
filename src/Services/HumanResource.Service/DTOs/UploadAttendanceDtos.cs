namespace HumanResource.Service.DTOs
{
    public sealed record UploadAttendanceLookupOptionV3Dto(
        int Id,
        string Code,
        string Name,
        string? Description
    );

    public sealed record UploadAttendanceLookupsV3Dto(
        IReadOnlyList<UploadAttendanceLookupOptionV3Dto> MemberTypes,
        IReadOnlyList<UploadAttendanceLookupOptionV3Dto> DeviceTypes,
        IReadOnlyList<UploadAttendanceLookupOptionV3Dto> FileTypes
    );

    public sealed record UploadAttendanceEmployeeV3Dto(
        int Id,
        string EmployeeCode,
        string EmployeeName,
        string Department,
        string Designation,
        string MachineCode
    );

    public sealed record UploadAttendanceProcessRowV3Request(
        string EmployeeCode,
        DateTime AttendanceDate,
        string? CheckIn,
        string? CheckOut,
        string? Remarks
    );

    public sealed record UploadAttendanceProcessV3Request(
        int? TypeId,
        int? DeviceId,
        int? FileTypeId,
        DateTime? FromDate,
        DateTime? TillDate,
        IReadOnlyList<UploadAttendanceProcessRowV3Request> Items
    );

    public sealed record UploadAttendanceProcessRowResultV3Dto(
        int RowNo,
        string EmployeeCode,
        bool Success,
        string Message,
        int? AttendanceId
    );

    public sealed record UploadAttendanceProcessResultV3Dto(
        int TotalCount,
        int SuccessCount,
        int FailedCount,
        IReadOnlyList<UploadAttendanceProcessRowResultV3Dto> Items
    );
}
