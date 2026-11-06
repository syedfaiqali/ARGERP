using HumanResource.Service.DTOs;

using MediatR;

namespace HumanResource.Service.Queries
{
    public record GetCalendarHolidaysV3Query(
        int PageNumber,
        int PageSize,
        string? SearchTerm,
        int ClientId,
        int? EntityId
    ) : IRequest<PaginationResponse<CalendarHolidayV3Dto>>;

    public record GetCalendarHolidayByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<CalendarHolidayV3Dto?>;
}
