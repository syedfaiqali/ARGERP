using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateAuthorityV3Command(
        CreateAuthorityV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<AuthorityV3Dto>;

    public record UpdateAuthorityV3Command(
        UpdateAuthorityV3Request Request,
        int ClientId,
        int? EntityId) : IRequest<AuthorityV3Dto>;

    public record DeleteAuthorityV3Command(
        int Id,
        int ClientId,
        int? EntityId) : IRequest<bool>;
}
