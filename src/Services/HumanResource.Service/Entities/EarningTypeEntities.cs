namespace HumanResource.Service.Entities
{
    public sealed class EPEarningtypeLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public string? Ltxt { get; set; }
        public int? PayRunTypeId { get; set; }
        public int? TaxMethodId { get; set; }
        public int? UomId { get; set; }
        public int? RateBasisId { get; set; }
        public bool? BsSalary { get; set; }
        public int? EarningTypeId { get; set; }
        public string? PaySlipIndex { get; set; }
        public string? PaySlipDescription { get; set; }
        public decimal? Value { get; set; }
        public int? ClientId { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
    }
}
