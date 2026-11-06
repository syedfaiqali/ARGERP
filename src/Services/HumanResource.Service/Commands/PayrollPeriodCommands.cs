using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreatePayrollPeriodV3Command(int ClientId, int? EntityId, CreatePayrollPeriodV3Request Request) : IRequest<int>;
    public record UpdatePayrollPeriodV3Command(int ClientId, int? EntityId, UpdatePayrollPeriodV3Request Request) : IRequest<bool>;
    public record DeletePayrollPeriodV3Command(int Id) : IRequest<bool>;
}
