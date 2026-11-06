using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateAttendancePolicyV3Command(CreateAttendancePolicyV3Request Request, int ClientId, int? EntityId) : IRequest<AttendancePolicyV3Dto>;
    public sealed record UpdateAttendancePolicyV3Command(UpdateAttendancePolicyV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteAttendancePolicyV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
