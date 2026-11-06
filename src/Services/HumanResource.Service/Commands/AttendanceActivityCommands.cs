using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateAttendanceActivityV3Command(
        CreateAttendanceActivityV3Request Request,
        int ClientId
    ) : IRequest<AttendanceActivityV3Dto>;

    public sealed record UpdateAttendanceActivityV3Command(
        UpdateAttendanceActivityV3Request Request,
        int ClientId
    ) : IRequest<bool>;

    public sealed record DeleteAttendanceActivityV3Command(
        int Id,
        int ClientId
    ) : IRequest<bool>;
}