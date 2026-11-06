using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed record CreateDepartmentV3Command(
        CreateDepartmentV3Request Request,
        int ClientId) : IRequest<DepartmentV3Dto>;

    public sealed record UpdateDepartmentV3Command(
        UpdateDepartmentV3Request Request,
        int ClientId) : IRequest<bool>;

    public sealed record DeleteDepartmentV3Command(int Id, int ClientId) : IRequest<MasterDeleteResult>;
}
