namespace HumanResource.Service.PayrollAccounting.Contracts;

public enum JvTrailKind
{
	Post,
	Reversal
}

/// <summary>
/// One JV in the history of a source document.
/// </summary>
/// <param name="IsActive">True for the JV currently representing the document (posted and not reversed)</param>
/// <param name="Version">FITransactionMaster.utrecordversion; greater than 1 means the JV was overwritten by a document edit</param>
public sealed record JvTrailItem(
	int TransactionId,
	string Code,
	DateTime TransactionDate,
	decimal Amount,
	JvTrailKind Kind,
	bool IsActive,
	int Version,
	string? Narration,
	DateTime? AddedOn,
	string? AddedBy,
	DateTime? EditedOn,
	string? EditedBy);

public enum JvPostingState
{
	NotPosted,
	Posted,
	Reversed
}

public sealed record JvPostingStatus(JvPostingState State, int? TransactionId, string? Code);
