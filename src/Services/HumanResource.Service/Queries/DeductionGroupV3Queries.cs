using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetDeductionGroupsV3Query(int ClientId, int? EntityId, string? SearchTerm = null) : IRequest<List<DeductionGroupV3Dto>>;
    public sealed record GetDeductionGroupByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<DeductionGroupHeaderV3Dto?>;
    public sealed record GetDeductionGroupLookupsV3Query(int ClientId, int? EntityId) : IRequest<DeductionGroupLookupsV3Dto>;
}
