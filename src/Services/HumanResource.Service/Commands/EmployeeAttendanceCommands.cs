using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Commands
{
    public sealed record CreateEmployeeAttendanceV3Command(CreateEmployeeAttendanceV3Request Request, int ClientId, int? EntityId) : IRequest<EmployeeAttendanceV3Dto>;
    public sealed record UpdateEmployeeAttendanceV3Command(UpdateEmployeeAttendanceV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteEmployeeAttendanceV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
