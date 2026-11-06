using EDXS.Shared.Finance.Commands;
using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Finance.Engine;
using MediatR;

namespace HumanResource.Service.Posting
{
    /// <summary>Published inside the pay slip save transaction, on create and on every re-process.</summary>
    public sealed record PayrollProcessedEvent(int ProcessId, int ClientId) : INotification;

    /// <summary>
    /// Processed -> Post, re-processed -> overwrite the same JV. If the pay slip no longer has
    /// anything to post (gross 0, department accounts removed) its JV is reversed.
    /// </summary>
    public sealed class PayrollJvEventHandler : INotificationHandler<PayrollProcessedEvent>
    {
        private readonly PayrollJvBuilder _builder;
        private readonly JvPostingEngine _engine;

        public PayrollJvEventHandler(PayrollJvBuilder builder, JvPostingEngine engine)
        {
            _builder = builder;
            _engine = engine;
        }

        public async Task Handle(PayrollProcessedEvent notification, CancellationToken cancellationToken)
        {
            var process = await _builder.LoadProcessAsync(notification.ProcessId, notification.ClientId, cancellationToken);
            if (process == null) return;

            var posting = await _builder.BuildAsync(process, cancellationToken);
            if (posting != null)
            {
                await _engine.RepostAsync(new RepostJvCommand(posting), cancellationToken);
                return;
            }

            var sourceId = process.Id.ToString();
            if (await _engine.HasActiveAsync(notification.ClientId, JvSources.Payroll, sourceId, cancellationToken))
            {
                await _engine.ReverseAsync(new ReverseJvCommand(notification.ClientId, process.entityid ?? 0, JvSources.Payroll,
                    sourceId, DateTime.Today, "Pay slip no longer posts to finance"), cancellationToken);
            }
        }
    }
}
