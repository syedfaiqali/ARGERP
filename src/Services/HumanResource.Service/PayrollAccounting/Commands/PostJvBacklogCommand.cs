using HumanResource.Service.PayrollAccounting.Contracts;
using EDXS.Shared.Infrastructure.CQRS;

namespace HumanResource.Service.PayrollAccounting.Commands;

/// <summary>
/// Posts JVs for selected backlog documents. Each document is posted under its own savepoint,
/// so one failure does not undo the others; failures come back in the result.
/// </summary>
public sealed record PostJvBacklogCommand(
	int ClientId,
	int EntityId,
	string Source,
	IReadOnlyList<string> SourceIds,
	string? UserName = null,
	string? IpAddress = null) : ICommand<IReadOnlyList<JvBacklogPostResult>>;
