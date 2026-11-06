using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed record CreateDesignationV3Command(
        CreateDesignationV3Request Request,
        int ClientId) : IRequest<DesignationV3Dto>;

    public sealed record UpdateDesignationV3Command(
        UpdateDesignationV3Request Request,
        int ClientId) : IRequest<bool>;

    public sealed record DeleteDesignationV3Command(int Id, int ClientId) : IRequest<MasterDeleteResult>;
}
