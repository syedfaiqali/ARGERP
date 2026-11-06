using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateLeaveTypeV3Command(
        CreateLeaveTypeV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<LeaveTypeV3Dto>;

    public record UpdateLeaveTypeV3Command(
        UpdateLeaveTypeV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<bool>;

    public record DeleteLeaveTypeV3Command(
        int Id,
        int ClientId,
        int? EntityId) : IRequest<bool>;
}
