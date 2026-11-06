using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateDeductionGroupV3Command(CreateDeductionGroupV3Request Request, int ClientId, int? EntityId) : IRequest<DeductionGroupHeaderV3Dto>;
    public sealed record UpdateDeductionGroupV3Command(UpdateDeductionGroupV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteDeductionGroupV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
