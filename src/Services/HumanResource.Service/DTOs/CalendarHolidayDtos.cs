using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public record CalendarHolidayV3Dto(
        int Id,
        string Code,
        string CalendarHoliday,
        int? PayrollPeriodId,
        int? ApplyOnId,
        int? SharedWithId,
        string PayrollPeriod,
        string ApplyOn,
        string SharedWith,
        bool IsActive,
        List<CalendarHolidayDetailV3Dto> Details,
        List<CalendarHolidayApplyOnDetailV3Dto> ApplyOnDetails,
        List<CalendarHolidaySharedDetailV3Dto> SharedDetails
    );

    public record CalendarHolidayDetailV3Dto(
        int Id,
        int? HolidayTypeId,
        string Description,
        DateTime? StartDate,
        DateTime? EndDate,
        int? Days,
        int? PayPeriodDetailId,
        int? Year,
        string Period
    );

    public record CalendarHolidayApplyOnDetailV3Dto(
        int Id,
        int? DepartId,
        int? EmpTypeId
    );

    public record CalendarHolidaySharedDetailV3Dto(
        int Id,
        int? SharedWithId,
        int? ClientId,
        int? EntityId,
        int? RegionId
    );

    public record CreateCalendarHolidayV3Request(
        string Code,
        string CalendarHoliday,
        int? PayrollPeriodId,
        int? ApplyOnId,
        int? SharedWithId,
        bool IsActive,
        List<CreateCalendarHolidayDetailV3Request> Details,
        List<CreateCalendarHolidayApplyOnDetailV3Request> ApplyOnDetails,
        List<CreateCalendarHolidaySharedDetailV3Request> SharedDetails
    );

    public record CreateCalendarHolidayDetailV3Request(
        int? HolidayTypeId,
        string Description,
        DateTime? StartDate,
        DateTime? EndDate,
        int? Days,
        int? PayPeriodDetailId,
        int? Year,
        string Period
    );

    public record CreateCalendarHolidayApplyOnDetailV3Request(
        int? DepartId,
        int? EmpTypeId
    );

    public record CreateCalendarHolidaySharedDetailV3Request(
        int? SharedWithId,
        int? ClientId,
        int? EntityId,
        int? RegionId
    );

    public record UpdateCalendarHolidayV3Request(
        int Id,
        string Code,
        string CalendarHoliday,
        int? PayrollPeriodId,
        int? ApplyOnId,
        int? SharedWithId,
        bool IsActive,
        List<UpdateCalendarHolidayDetailV3Request> Details,
        List<UpdateCalendarHolidayApplyOnDetailV3Request> ApplyOnDetails,
        List<UpdateCalendarHolidaySharedDetailV3Request> SharedDetails
    );

    public record UpdateCalendarHolidayDetailV3Request(
        int? Id,
        int? HolidayTypeId,
        string Description,
        DateTime? StartDate,
        DateTime? EndDate,
        int? Days,
        int? PayPeriodDetailId,
        int? Year,
        string Period
    );

    public record UpdateCalendarHolidayApplyOnDetailV3Request(
        int? Id,
        int? DepartId,
        int? EmpTypeId
    );

    public record UpdateCalendarHolidaySharedDetailV3Request(
        int? Id,
        int? SharedWithId,
        int? ClientId,
        int? EntityId,
        int? RegionId
    );
}
