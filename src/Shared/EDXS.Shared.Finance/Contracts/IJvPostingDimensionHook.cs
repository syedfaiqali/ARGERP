using EDXS.Shared.Finance.Commands;

namespace EDXS.Shared.Finance.Contracts;

public sealed record JvPostedLine(int DetailId, JvLine Line);

/// <summary>
/// Optional extension point (FinancialManagement registers the project-dimension implementation).
/// Runs inside the posting transaction; an exception rolls the JV back.
/// </summary>
public interface IJvPostingDimensionHook
{
	/// <param name="currentTransactionId">The JV being overwritten, or null for a new JV</param>
	Task ValidateAsync(PostJvCommand posting, int? currentTransactionId, CancellationToken cancellationToken);

	Task OnLinesPostedAsync(PostJvCommand posting, int transactionId, IReadOnlyList<JvPostedLine> lines, CancellationToken cancellationToken);

	Task OnLinesRemovedAsync(IReadOnlyCollection<int> detailIds, CancellationToken cancellationToken);
}
