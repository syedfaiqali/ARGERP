namespace HumanResource.Service.Entities
{
    public sealed class EPDeductiontypeLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public string? Ltxt { get; set; }
        public int? PayRunTypeId { get; set; }
        public bool Reimbursed { get; set; }
        public int? ClientId { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
    }
}
