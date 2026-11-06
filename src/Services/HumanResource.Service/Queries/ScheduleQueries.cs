using HumanResource.Service.DTOs;
using MediatR;
using HumanResource.Service.Infrastructure.Data;

namespace HumanResource.Service.Queries
{
    public sealed record GetSchedulesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm = null) : IRequest<PaginationResponse<ScheduleV3Dto>>;

    public sealed record GetScheduleByIdV3Query(int Id, int ClientId, int? EntityId)
        : IRequest<ScheduleHeaderV3Dto?>;

    public sealed record GetScheduleDetailsV3Query(DateTime FromDate, DateTime TillDate)
        : IRequest<IReadOnlyList<ScheduleDetailV3Dto>>;
}
