using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateEmploymentV3Command(
        EmploymentUpsertV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<EmploymentV3Dto>;

    public sealed record UpdateEmploymentV3Command(
        EmploymentUpsertV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;

    public sealed record DeleteEmploymentV3Command(
        int Id,
        int ClientId,
        int? EntityId
    ) : IRequest<bool>;
}
