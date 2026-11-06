using HumanResource.Service.PayrollAccounting.Contracts;
using EDXS.Shared.Infrastructure.CQRS;

namespace HumanResource.Service.PayrollAccounting.Commands;

/// <summary>
/// Document voided: inserts a "-R" JV with the lines swapped. The original JV is left intact.
/// </summary>
/// <param name="ReversalDate">Posting date of the reversal; defaults to the original JV date</param>
public sealed record ReverseJvCommand(
	int ClientId,
	int EntityId,
	string Source,
	string SourceId,
	DateTime? ReversalDate,
	string? Reason,
	string? UserName = null,
	string? IpAddress = null) : ICommand<JvPostingResult>;
