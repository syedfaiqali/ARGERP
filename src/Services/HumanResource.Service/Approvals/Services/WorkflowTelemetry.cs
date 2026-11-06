using System.Diagnostics.Metrics;

namespace HumanResource.Service.Approvals.Services;

public static class WorkflowTelemetry
{
    public static readonly Meter Meter = new("EDXS.Workflow", "1.0.0");
    public static readonly Counter<long> Submissions = Meter.CreateCounter<long>("workflow.submissions");
    public static readonly Counter<long> Decisions = Meter.CreateCounter<long>("workflow.decisions");
    public static readonly Counter<long> ExecutionAttempts = Meter.CreateCounter<long>("workflow.execution.attempts");
    public static readonly Histogram<double> CallbackDuration = Meter.CreateHistogram<double>("workflow.execution.callback.duration", "ms");
}
