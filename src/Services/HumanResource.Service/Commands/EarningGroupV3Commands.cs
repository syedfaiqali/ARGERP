using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateEarningGroupV3Command(CreateEarningGroupV3Request Request, int ClientId, int? EntityId) : IRequest<EarningGroupHeaderV3Dto>;
    public sealed record UpdateEarningGroupV3Command(UpdateEarningGroupV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteEarningGroupV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
