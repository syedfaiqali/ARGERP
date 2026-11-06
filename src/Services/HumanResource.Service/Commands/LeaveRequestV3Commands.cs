using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateLeaveRequestV3Command(
        CreateLeaveRequestV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<LeaveRequestV3Dto>;

    public sealed record UpdateLeaveRequestV3Command(
        UpdateLeaveRequestV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<bool>;

    public sealed record DeleteLeaveRequestV3Command(
        int Id,
        int ClientId,
        int? EntityId) : IRequest<bool>;
}
