namespace HumanResource.Service.Entities;

// Read-only projections of the existing Workflow Management tables. No new tables or approval state.
public sealed class LoanWorkflowApprovalInstance
{
    public Guid Id { get; set; }
    public int ClientId { get; set; }
    public int? EntityId { get; set; }
    public string ScreenKey { get; set; } = "";
    public string Operation { get; set; } = "";
    public string Status { get; set; } = "";
    public string? RecordKey { get; set; }
}

public sealed class LoanWorkflowApprovalTask
{
    public Guid Id { get; set; }
    public Guid InstanceId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string? Decision { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
}

public sealed class LoanWorkflowApprovalEvent
{
    public long Id { get; set; }
    public Guid? InstanceId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public string Kind { get; set; } = "";
    public string Comment { get; set; } = "";
    public DateTime AtUtc { get; set; }
}
