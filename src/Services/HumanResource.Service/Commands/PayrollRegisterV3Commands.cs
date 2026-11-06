using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreatePayrollRegisterV3Command(CreatePayrollRegisterV3Request Request, int ClientId) : IRequest<PayrollRegisterV3Dto>;
    public sealed record UpdatePayrollRegisterV3Command(UpdatePayrollRegisterV3Request Request, int ClientId) : IRequest<bool>;
    public sealed record DeletePayrollRegisterV3Command(int Id, int ClientId) : IRequest<bool>;
}
