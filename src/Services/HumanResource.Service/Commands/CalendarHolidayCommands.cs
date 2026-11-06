using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateCalendarHolidayV3Command(CreateCalendarHolidayV3Request Request, int ClientId, int? EntityId) : IRequest<CalendarHolidayV3Dto>;
    public record UpdateCalendarHolidayV3Command(UpdateCalendarHolidayV3Request Request, int ClientId, int? EntityId) : IRequest<CalendarHolidayV3Dto>;
    public record DeleteCalendarHolidayV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
