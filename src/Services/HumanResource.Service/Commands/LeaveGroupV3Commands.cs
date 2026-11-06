using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateLeaveGroupV3Command(LeaveGroupHeaderV3Dto Dto, int ClientId, int? EntityId) : IRequest<int>;
    public record UpdateLeaveGroupV3Command(LeaveGroupHeaderV3Dto Dto, int ClientId, int? EntityId) : IRequest<bool>;
    public record DeleteLeaveGroupV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
