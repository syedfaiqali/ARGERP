using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Infrastructure.CQRS;

namespace EDXS.Shared.Finance.Commands;

/// <summary>
/// Inserts a new JV (vouchertypeid = 1). A system JV (Source set) is rejected when the
/// source already has an active JV; use <see cref="RepostJvCommand"/> for document edits.
/// </summary>
public sealed record PostJvCommand : ICommand<JvPostingResult>
{
	public int ClientId { get; init; }
	public int EntityId { get; init; }

	/// <summary>Source code from <see cref="JvSources"/>; null for a manual JV.</summary>
	public string? Source { get; init; }
	public string? SourceId { get; init; }

	public DateTime TransactionDate { get; init; }
	public string? Narration { get; init; }
	public IReadOnlyList<JvLine> Lines { get; init; } = [];

	/// <summary>utsysgen. True for document postings, false for the manual JV screen.</summary>
	public bool SystemGenerated { get; init; } = true;

	/// <summary>Apply the FIPolicyhdr back-date rules. Off by default for system postings.</summary>
	public bool EnforceBackDatePolicy { get; init; }

	/// <summary>Manual JV only: a user-entered code. Left empty, the engine assigns JV-00001 style codes.</summary>
	public string? Code { get; init; }

	public string? UserName { get; init; }
	public string? IpAddress { get; init; }
}
