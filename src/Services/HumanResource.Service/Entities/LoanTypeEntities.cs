namespace HumanResource.Service.Entities
{
    public sealed class EPLoantypeLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public string? ltxt { get; set; }
        public int? loannatureid { get; set; }
        public int? limitid { get; set; }
        public double? limit { get; set; }
        public int? availid { get; set; }
        public int? loaninterestid { get; set; }
        public int? jobduration { get; set; }
        public int? noofloan { get; set; }
        public int? noofinstallments { get; set; }
        public bool flexible { get; set; }
        public int? clientid { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }
    }
}
