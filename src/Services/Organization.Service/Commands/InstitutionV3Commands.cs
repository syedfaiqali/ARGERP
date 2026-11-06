using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed record CreateInstitutionV3Command(
        SaveInstitutionV3Request Request,
        int ClientId) : IRequest<InstitutionV3Dto>;

    public sealed record UpdateInstitutionV3Command(
        SaveInstitutionV3Request Request,
        int ClientId) : IRequest<bool>;

    public sealed record DeleteInstitutionV3Command(
        int Id,
        int ClientId) : IRequest<bool>;
}
