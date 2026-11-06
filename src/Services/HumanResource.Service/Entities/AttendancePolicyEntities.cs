using System.Collections.Generic;

namespace HumanResource.Service.Entities
{
    public class ETPolicyhdrLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public int? Policytypeid { get; set; }
        public bool? Applyoffdays { get; set; }
        public int? Noofexcuse { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool UtSysGen { get; set; } = false;
        public int UtRecordVersion { get; set; } = 0;
        public bool UtIsPartial { get; set; } = false;
        public bool Utblock { get; set; }

        public List<ETPolicydtlLegacy> PolicyDetails { get; set; } = new();
        public List<ETAbsentpolicydtlLegacy> AbsentDetails { get; set; } = new();
    }

    public class ETPolicydtlLegacy
    {
        public int Id { get; set; }
        public int? Policyid { get; set; }
        public int? Lineindex { get; set; }
        public int? Starttime { get; set; }
        public int? Endtime { get; set; }
        public int? Periodtypeid { get; set; }
        public int? Valtypeid { get; set; }
        public double? Value { get; set; }
        public bool UtSysGen { get; set; } = false;
        public int UtRecordVersion { get; set; } = 0;
        public bool UtIsPartial { get; set; } = false;
        public bool Utblock { get; set; }

        public ETPolicyhdrLegacy Header { get; set; } = null!;
    }

    public class ETAbsentpolicydtlLegacy
    {
        public int Id { get; set; }
        public int? Policyid { get; set; }
        public int? Lineindex { get; set; }
        public int? Periodtypeid { get; set; }
        public double? Policycount { get; set; }
        public double? Absentcount { get; set; }
        public bool UtSysGen { get; set; } = false;
        public int UtRecordVersion { get; set; } = 0;
        public bool UtIsPartial { get; set; } = false;
        public bool Utblock { get; set; }

        public ETPolicyhdrLegacy Header { get; set; } = null!;
    }
}
