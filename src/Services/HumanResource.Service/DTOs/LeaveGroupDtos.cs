namespace HumanResource.Service.DTOs
{
    public class LeaveGroupV3Dto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public int? AdjustmentPolicyId { get; set; }
        public string? AdjustmentPolicyName { get; set; }
    }

    public class LeaveGroupHeaderV3Dto : LeaveGroupV3Dto
    {
        public List<LeaveGroupDetailV3Dto> Details { get; set; } = new List<LeaveGroupDetailV3Dto>();
    }

    public class LeaveGroupDetailV3Dto
    {
        public int Id { get; set; }
        public int? LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? LeaveTypeCode { get; set; }
        public int? LeaveDays { get; set; }
    }
}
