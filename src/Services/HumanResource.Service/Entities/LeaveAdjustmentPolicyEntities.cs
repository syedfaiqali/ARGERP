using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HumanResource.Service.Entities
{
    public class ETLeaveadjpolicyLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public bool utblock { get; set; }
        public bool utsysgen { get; set; } = false;
        public int utrecordversion { get; set; } = 0;
        public bool utispartial { get; set; } = false;

        public ICollection<ETLeaveadjpolicydtlLegacy> Details { get; set; } = new List<ETLeaveadjpolicydtlLegacy>();
    }

    public class ETLeaveadjpolicydtlLegacy
    {
        public int Id { get; set; }
        public int? leaveadjid { get; set; }
        public int? leavetypeid { get; set; }
        public int? adjtypeid { get; set; }
        public int? basedonid { get; set; }
        public int? priorityid { get; set; }
        public int days { get; set; }
        public bool utblock { get; set; }
        public bool utsysgen { get; set; } = false;
        public int utrecordversion { get; set; } = 0;
        public bool utispartial { get; set; } = false;

        [JsonIgnore]
        public ETLeaveadjpolicyLegacy? Header { get; set; }
    }

    public class ETLeavetypeLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public double? leaves { get; set; }
        public int? renewonid { get; set; }
        public int? lvunitid { get; set; }
        public int? availunitid { get; set; }
        public int? maxavail { get; set; }
        public int? maritalid { get; set; }
        public double? carrylv { get; set; }
        public int? carrylvid { get; set; }
        public double? encash { get; set; }
        public int? encashid { get; set; }
        public int? genderid { get; set; }
        public int? entitleid { get; set; }
        public int? accrualunitid { get; set; }
        public double? accrualleaves { get; set; }
        public bool? allowinprob { get; set; }
        public bool? quotavalidate { get; set; }
        public bool? lateadjust { get; set; }
        public bool? earlydepadjust { get; set; }
        public bool? halfadjust { get; set; }
        public bool? incholidays { get; set; }
        public bool? exclweeklyholidays { get; set; }
        public bool? ispaid { get; set; }
        public int? requestbefdays { get; set; }
        public int? requestunitid { get; set; }
        public int? nom { get; set; }
        public bool? utsysgen { get; set; } = false;
        public bool? utblock { get; set; }
        public bool? utispartial { get; set; } = false;
        public int? utrecordversion { get; set; } = 0;
    }
}
