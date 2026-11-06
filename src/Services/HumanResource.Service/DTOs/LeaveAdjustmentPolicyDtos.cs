namespace HumanResource.Service.DTOs
{
    public class LeaveAdjustmentPolicyV3Dto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public List<LeaveAdjustmentPolicyDetailV3Dto> Details { get; set; } = new();
    }

    public class LeaveAdjustmentPolicyDetailV3Dto
    {
        public int Id { get; set; }
        public int? LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public int? AdjTypeId { get; set; }
        public string? AdjTypeName { get; set; }
        public int? BasedOnId { get; set; }
        public string? BasedOnName { get; set; }
        public int? PriorityId { get; set; }
        public string? PriorityName { get; set; }
        public int Days { get; set; }
    }

    public class CreateLeaveAdjustmentPolicyV3Request
    {
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public List<CreateLeaveAdjustmentPolicyDetailV3Request> Details { get; set; } = new();
    }

    public class CreateLeaveAdjustmentPolicyDetailV3Request
    {
        public int? LeaveTypeId { get; set; }
        public int? AdjTypeId { get; set; }
        public int? BasedOnId { get; set; }
        public int? PriorityId { get; set; }
        public int Days { get; set; }
    }

    public class UpdateLeaveAdjustmentPolicyV3Request
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public List<UpdateLeaveAdjustmentPolicyDetailV3Request> Details { get; set; } = new();
    }

    public class UpdateLeaveAdjustmentPolicyDetailV3Request
    {
        public int? Id { get; set; } // Null if new detail
        public int? LeaveTypeId { get; set; }
        public int? AdjTypeId { get; set; }
        public int? BasedOnId { get; set; }
        public int? PriorityId { get; set; }
        public int Days { get; set; }
        public bool Delete { get; set; }
    }
}
