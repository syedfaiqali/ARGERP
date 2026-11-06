using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetAttendanceActivitiesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        string? SearchTerm
    ) : IRequest<PaginationResponse<AttendanceActivityV3Dto>>;

    public sealed record GetAttendanceActivityByIdV3Query(
        int Id,
        int ClientId
    ) : IRequest<AttendanceActivityV3Dto?>;
}