using EDXS.Shared.Workflow;

namespace HumanResource.Service.Approvals.Services;

// Approval execution retains its signed callback identity while running inside HR.
public sealed class HrApprovalClient(WorkflowHttpClient client)
{
    public Task<T> PostAsync<T>(string destination, string path, object body, CancellationToken ct) =>
        client.PostAsync<T>(destination, path, body, ct, sender: "workflow");
}
