using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateLeaveTransactionV3Command(CreateLeaveTransactionV3Dto Request, int? EntityId, int? ClientId) : IRequest<LeaveTransactionV3Dto>;

    public record UpdateLeaveTransactionV3Command(int Id, CreateLeaveTransactionV3Dto Request) : IRequest<bool>;

    public record DeleteLeaveTransactionV3Command(int Id) : IRequest<bool>;
}
