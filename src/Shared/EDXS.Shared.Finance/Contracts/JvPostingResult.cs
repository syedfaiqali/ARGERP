namespace EDXS.Shared.Finance.Contracts;

public enum JvPostingAction
{
	/// <summary>A new JV was inserted.</summary>
	Posted,
	/// <summary>The existing JV was overwritten in place (same Id and code, new lines).</summary>
	Overwritten,
	/// <summary>A "-R" reversal JV was inserted; the original is left intact.</summary>
	Reversed
}

/// <param name="TransactionId">FITransactionMaster.Id of the JV that was written</param>
/// <param name="ReversedTransactionId">For a reversal, the Id of the original JV</param>
public sealed record JvPostingResult(int TransactionId, string Code, JvPostingAction Action, int? ReversedTransactionId = null);
