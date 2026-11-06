namespace Organization.Service.DTOs
{
    public class CreateFOCalllogV3Request
    {
        public DateTime? CallDate { get; set; }
        public DateTime? CallTime { get; set; }
        public int? PurposeTypeId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public int? CallTypeId { get; set; }
        public int? CallMediumId { get; set; }
        public string? Duration { get; set; }
        public string? Description { get; set; }
        public int EntityId { get; set; }
    }
}
