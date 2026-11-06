using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetAttendancePoliciesV3Query(int ClientId, int? EntityId, string? SearchTerm = null) : IRequest<List<AttendancePolicyV3Dto>>;
    public sealed record GetAttendancePolicyByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<AttendancePolicyHeaderV3Dto?>;
}
