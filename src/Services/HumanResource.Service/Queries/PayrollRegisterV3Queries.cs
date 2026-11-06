using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetPayrollRegistersV3Query(int ClientId, string? SearchTerm = null) : IRequest<List<PayrollRegisterV3Dto>>;
    public sealed record GetPayrollRegisterByIdV3Query(int Id, int ClientId) : IRequest<PayrollRegisterV3Dto?>;
}
