using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetEarningTypesV3Query(int ClientId, int? EntityId, string? SearchTerm = null) : IRequest<List<EarningTypeV3Dto>>;
    public sealed record GetEarningTypeByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<EarningTypeV3Dto?>;
    public sealed record GetEarningTypeLookupsV3Query(int ClientId, int? EntityId) : IRequest<EarningTypeLookupsV3Dto>;
}
