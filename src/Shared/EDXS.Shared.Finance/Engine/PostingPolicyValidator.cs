using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Finance.Data;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Finance.Engine;

/// <summary>
/// FIPolicyhdr back-date rules (moved from FinancialManagement's FinancePolicyValidator).
/// </summary>
public sealed class PostingPolicyValidator
{
	private readonly TimeProvider _clock;

	public PostingPolicyValidator(TimeProvider clock)
	{
		_clock = clock;
	}

	public async Task ValidateTransactionDateAsync(
		JvPostingDbContext db,
		DateTime transactionDate,
		int clientId,
		int entityId,
		DateTime? existingTransactionDate,
		CancellationToken cancellationToken)
	{
		var policy = await db.Policies
			.AsNoTracking()
			.Where(x => x.EntityId == entityId && (clientId == 0 || x.ClientId == clientId || x.ClientId == null))
			.OrderByDescending(x => x.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (policy == null)
		{
			return;
		}

		var today = _clock.GetLocalNow().Date;
		if (!policy.TransactionAllowInBackDate && transactionDate.Date < today)
		{
			throw new JvPostingException("Back date transactions are not allowed by system policy.");
		}

		if (!policy.CanChangeTransactionDate
			&& existingTransactionDate.HasValue
			&& existingTransactionDate.Value.Date != transactionDate.Date)
		{
			throw new JvPostingException("Transaction date cannot be changed by system policy.");
		}
	}
}
