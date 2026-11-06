using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Finance.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EDXS.Shared.Finance.Engine;

/// <summary>
/// Serialises FITransactionMaster/Detail Id and voucher code allocation across all services.
/// The lock is owned by the current transaction and released on commit/rollback.
/// </summary>
public interface IFiPostingLock
{
	Task AcquireAsync(DbContext db, CancellationToken cancellationToken);
}

public sealed class SqlServerFiPostingLock : IFiPostingLock
{
	public const string Resource = "EDXS_FI_POSTING";
	private const int TimeoutMilliseconds = 15000;

	public async Task AcquireAsync(DbContext db, CancellationToken cancellationToken)
	{
		var transaction = db.Database.CurrentTransaction
			?? throw new InvalidOperationException("The FI posting lock must be taken inside a transaction.");

		var connection = db.Database.GetDbConnection();
		await using var command = connection.CreateCommand();
		command.Transaction = transaction.GetDbTransaction();
		command.CommandText =
			"DECLARE @result int; " +
			"EXEC @result = sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = @timeout; " +
			"SELECT @result;";

		var resource = command.CreateParameter();
		resource.ParameterName = "@resource";
		resource.Value = Resource;
		command.Parameters.Add(resource);

		var timeout = command.CreateParameter();
		timeout.ParameterName = "@timeout";
		timeout.Value = TimeoutMilliseconds;
		command.Parameters.Add(timeout);

		var result = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
		if (result < 0)
		{
			throw new JvPostingException(result == -1
				? "Another finance posting is in progress. Please try again."
				: $"Could not acquire the finance posting lock (code {result}).");
		}
	}
}

/// <summary>
/// Next Ids and JV codes. Call only after <see cref="IFiPostingLock.AcquireAsync"/>.
/// </summary>
public static class FiPostingNumbers
{
	private const string JvPrefix = "JV-";

	public static async Task<int> NextMasterIdAsync(JvPostingDbContext db, CancellationToken cancellationToken) =>
		(await db.TransactionMasters.MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

	public static async Task<int> NextDetailIdAsync(JvPostingDbContext db, CancellationToken cancellationToken) =>
		(await db.TransactionDetails.MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

	/// <summary>JV-00001 style, per client. Reversal ("-R") and non-numeric codes are ignored.</summary>
	public static async Task<string> NextJvCodeAsync(JvPostingDbContext db, int clientId, CancellationToken cancellationToken)
	{
		var codes = await db.TransactionMasters
			.Where(x => x.ClientId == clientId
				&& x.VoucherTypeId == JvReference.JvVoucherTypeId
				&& x.Code != null
				&& x.Code.StartsWith(JvPrefix)
				&& !x.Code.EndsWith(JvReference.ReversalSuffix))
			.Select(x => x.Code!)
			.ToListAsync(cancellationToken);

		var last = codes
			.Select(code => int.TryParse(code.AsSpan(JvPrefix.Length), out var number) ? number : 0)
			.DefaultIfEmpty(0)
			.Max();

		return $"{JvPrefix}{last + 1:D5}";
	}
}
