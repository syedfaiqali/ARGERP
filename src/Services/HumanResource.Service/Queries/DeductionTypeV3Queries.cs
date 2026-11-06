using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetDeductionTypesV3Query(int ClientId, int? EntityId, string? SearchTerm = null) : IRequest<List<DeductionTypeV3Dto>>;
    public sealed record GetDeductionTypeByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<DeductionTypeV3Dto?>;
    public sealed record GetDeductionTypeLookupsV3Query(int ClientId, int? EntityId) : IRequest<DeductionTypeLookupsV3Dto>;
}
