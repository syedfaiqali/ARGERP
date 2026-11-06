using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public record PayrollPeriodV3Dto(
        int Id,
        string Code,
        string Ltxt,
        string? PayrollRegisterName,
        string? AttendaneTypeName,
        DateTime? FromDate,
        int? NoOfPeriod,
        string? StatusName
    );

    public record PayrollPeriodHeaderV3Dto(
        int Id,
        string Code,
        DateTime? Date,
        int? Atntypeid,
        DateTime? Fromyear,
        DateTime? Toyear,
        string Ltxt,
        int? Startfrom,
        int? Statusid,
        int? Lovsharedid,
        DateTime? Startdate,
        int? Payrollregisterid,
        int? Noofperiod,
        bool IsActive,
        List<PayrollPeriodDetailV3Dto> Details
    );

    public record PayrollPeriodDetailV3Dto(
        int Id,
        int? PayPeriodId,
        string Periodstxt,
        int? Year,
        DateTime? StartDate,
        DateTime? EndDate,
        int? NoOfDays,
        DateTime? DisbDate,
        int? StatusId,
        int? LineIndex
    );

    public record NextPayrollRegisterDateV3Dto(
        DateTime? Startdate
    );

    public record CreatePayrollPeriodV3Request(
        string Code,
        DateTime? Date,
        int? Atntypeid,
        DateTime? Fromyear,
        DateTime? Toyear,
        string Ltxt,
        int? Startfrom,
        int? Statusid,
        int? Lovsharedid,
        DateTime? Startdate,
        int? Payrollregisterid,
        int? Noofperiod,
        bool IsActive,
        List<CreatePayrollPeriodDetailV3Request> Details
    );

    public record CreatePayrollPeriodDetailV3Request(
        string Periodstxt,
        int? Year,
        DateTime? StartDate,
        DateTime? EndDate,
        int? NoOfDays,
        DateTime? DisbDate,
        int? StatusId,
        int? LineIndex
    );

    public record UpdatePayrollPeriodV3Request(
        int Id,
        string Code,
        DateTime? Date,
        int? Atntypeid,
        DateTime? Fromyear,
        DateTime? Toyear,
        string Ltxt,
        int? Startfrom,
        int? Statusid,
        int? Lovsharedid,
        DateTime? Startdate,
        int? Payrollregisterid,
        int? Noofperiod,
        bool IsActive,
        List<UpdatePayrollPeriodDetailV3Request> Details
    );

    public record UpdatePayrollPeriodDetailV3Request(
        int? Id,
        string Periodstxt,
        int? Year,
        DateTime? StartDate,
        DateTime? EndDate,
        int? NoOfDays,
        DateTime? DisbDate,
        int? StatusId,
        int? LineIndex
    );
}
