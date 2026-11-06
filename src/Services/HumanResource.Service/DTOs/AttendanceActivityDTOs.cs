namespace HumanResource.Service.DTOs;

public sealed record AttendanceActivityV3Dto(
    int Id,
    int? OrderId,
    string? Activity,
    int? Value
);

public sealed record CreateAttendanceActivityV3Request(
    int? OrderId,
    string? Activity,
    int? Value
);

public sealed record UpdateAttendanceActivityV3Request(
    int Id,
    int? OrderId,
    string? Activity,
    int? Value
);