using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class SMDevicetypeLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? ClientId { get; set; }
        public bool UtSysGen { get; set; } = false;
        public bool UtIsPartial { get; set; } = false;
        public int UtRecordVersion { get; set; } = 0;
        public bool UtBlock { get; set; } = false;
    }
}
