using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Finance;

/// <summary>
/// Runs work inside a database transaction that the JV engine can join.
/// Reuses the caller's open transaction; otherwise opens one inside the context's execution
/// strategy, so it also works with EnableRetryOnFailure (where a bare BeginTransaction throws).
/// </summary>
public static class FinanceTransaction
{
	public static async Task<T> ExecuteAsync<T>(DbContext db, Func<CancellationToken, Task<T>> work, CancellationToken cancellationToken)
	{
		// Non-relational providers (unit tests on InMemory) have no transactions to join.
		if (db.Database.CurrentTransaction != null || !db.Database.IsRelational())
		{
			return await work(cancellationToken);
		}

		var attempt = 0;
		var strategy = db.Database.CreateExecutionStrategy();
		return await strategy.ExecuteAsync(async () =>
		{
			// A retried attempt must not re-save entities staged by the failed one.
			if (attempt++ > 0) db.ChangeTracker.Clear();

			await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
			var result = await work(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
			return result;
		});
	}

	public static Task ExecuteAsync(DbContext db, Func<CancellationToken, Task> work, CancellationToken cancellationToken) =>
		ExecuteAsync(db, async ct =>
		{
			await work(ct);
			return true;
		}, cancellationToken);
}
