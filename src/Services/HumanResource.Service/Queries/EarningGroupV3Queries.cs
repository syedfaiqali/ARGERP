using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetEarningGroupsV3Query(int ClientId, int? EntityId, string? SearchTerm = null) : IRequest<List<EarningGroupV3Dto>>;
    public sealed record GetEarningGroupByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<EarningGroupHeaderV3Dto?>;
    public sealed record GetEarningGroupLookupsV3Query(int ClientId, int? EntityId) : IRequest<EarningGroupLookupsV3Dto>;
}
