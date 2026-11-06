using System;
using System.Collections.Generic;

namespace HumanResource.Service.Entities
{
    public class ETCalholidayhdrLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Stxt { get; set; } = string.Empty;
        public int? Payperiodid { get; set; }
        public int? Applyonid { get; set; }
        public int? Lovsharedid { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool Utblock { get; set; }

        public ICollection<ETCalholidaydtlLegacy> Details { get; set; } = new List<ETCalholidaydtlLegacy>();
        public ICollection<ETCalholidayapplyondtlLegacy> ApplyOnDetails { get; set; } = new List<ETCalholidayapplyondtlLegacy>();
        public ICollection<ETCalholidayshareddtlLegacy> SharedDetails { get; set; } = new List<ETCalholidayshareddtlLegacy>();
    }

    public class ETCalholidaydtlLegacy
    {
        public int Id { get; set; }
        public int? Calholidayid { get; set; }
        public int? Lineindex { get; set; }
        public int? Holidaytypeid { get; set; }
        public string? Ltxt { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public int? Days { get; set; }
        public int? Payperioddtlid { get; set; }
        public bool Utblock { get; set; }
        public ETCalholidayhdrLegacy Header { get; set; } = null!;
    }

    public class ETCalholidayapplyondtlLegacy
    {
        public int Id { get; set; }
        public int? Calholidayid { get; set; }
        public int? Lineindex { get; set; }
        public int? Departid { get; set; }
        public int? Emptypeid { get; set; }
        public bool Utblock { get; set; }
        public ETCalholidayhdrLegacy Header { get; set; } = null!;
    }

    public class ETCalholidayshareddtlLegacy
    {
        public int Id { get; set; }
        public int? Calholidayid { get; set; }
        public int? Lineindex { get; set; }
        public int? Lovsharedid { get; set; }
        public int? Pclientid { get; set; }
        public int? Pentityid { get; set; }
        public int? Regionid { get; set; }
        public bool Utblock { get; set; }
        public ETCalholidayhdrLegacy Header { get; set; } = null!;
    }

    public class UtLovLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public string? Ltxt { get; set; }
        public string? Vtype { get; set; }
        public bool UtBlock { get; set; }
    }

    public class ETPayperiodhdrLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public DateTime? Date { get; set; }
        public int? Atntypeid { get; set; }
        public DateTime? Fromyear { get; set; }
        public DateTime? Toyear { get; set; }
        public string? Ltxt { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public int? Startfrom { get; set; }
        public int? Statusid { get; set; }
        public int? Lovsharedid { get; set; }
        public DateTime? Startdate { get; set; }
        public int? Payrollregisterid { get; set; }
        public int? Noofperiod { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 1;
        public bool Utblock { get; set; }
        public List<ETPayperioddtlLegacy> Details { get; set; } = new();
    }

    public class ETPayperioddtlLegacy
    {
        public int Id { get; set; }
        public int? Payperiodid { get; set; }
        public int? Year { get; set; }
        public int? Monthid { get; set; }
        public string? Periodstxt { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public int? Noofdays { get; set; }
        public DateTime? Disbdate { get; set; }
        public int? Statusid { get; set; }
        public int? Lineindex { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 1;
        public bool utblock { get; set; }
        public ETPayperiodhdrLegacy Header { get; set; } = null!;
    }

    public class ETPayrollregisterLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public string? Ltxt { get; set; }
        public int? ClientId { get; set; }
        public bool Utblock { get; set; }
    }
}
