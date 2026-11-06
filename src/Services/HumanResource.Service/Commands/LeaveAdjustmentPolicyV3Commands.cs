using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateLeaveAdjustmentPolicyV3Command(
        CreateLeaveAdjustmentPolicyV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<LeaveAdjustmentPolicyV3Dto>;

    public record UpdateLeaveAdjustmentPolicyV3Command(
        UpdateLeaveAdjustmentPolicyV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<bool>;

    public record DeleteLeaveAdjustmentPolicyV3Command(
        int Id,
        int ClientId,
        int? EntityId) : IRequest<bool>;
}
