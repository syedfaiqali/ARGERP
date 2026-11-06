namespace Organization.Service.DTOs
{
    public class FOCalllogV3Dto
    {
        public int Id { get; set; }
        public string? LogId { get; set; }
        public string? CallDate { get; set; }
        public string? CallTime { get; set; }
        public int? PurposeTypeId { get; set; }
        public string? PurposeType { get; set; }
        public string? Name { get; set; }
        public string? PhoneNo { get; set; }
        public int? CallTypeId { get; set; }
        public string? CallType { get; set; }
        public int? CallMediumId { get; set; }
        public string? CallMedium { get; set; }
        public string? Duration { get; set; }
        public string? Description { get; set; }
        public int EntityId { get; set; }
    }
}
