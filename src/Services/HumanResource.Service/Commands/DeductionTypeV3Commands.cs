using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateDeductionTypeV3Command(CreateDeductionTypeV3Request Request, int ClientId, int? EntityId) : IRequest<DeductionTypeV3Dto>;
    public sealed record UpdateDeductionTypeV3Command(UpdateDeductionTypeV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteDeductionTypeV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
