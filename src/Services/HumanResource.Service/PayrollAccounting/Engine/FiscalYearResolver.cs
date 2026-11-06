using HumanResource.Service.PayrollAccounting.Contracts;
using HumanResource.Service.PayrollAccounting.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.PayrollAccounting.Engine;

/// <summary>
/// Finds the open FIFiscalYear (status = 1, utblock = 0) of a client/entity that contains a date.
/// </summary>
public sealed class FiscalYearResolver
{
	public async Task<int> ResolveOpenAsync(JvPostingDbContext db, int clientId, int entityId, DateTime date, CancellationToken cancellationToken)
	{
		var day = date.Date;
		var years = await db.FiscalYears
			.AsNoTracking()
			.Where(x => x.ClientId == clientId
				&& x.EntityId == entityId
				&& x.StartDate != null && x.EndDate != null
				&& x.StartDate <= day && x.EndDate >= day)
			.ToListAsync(cancellationToken);

		var open = years.FirstOrDefault(x => x.Status && !x.Blocked);
		if (open != null) return open.Id;

		if (years.Count > 0)
			throw new JvPostingException($"Fiscal year '{years[0].Title ?? years[0].Code}' is closed for {day:dd-MMM-yyyy}.");

		throw new JvPostingException($"No fiscal year is defined for {day:dd-MMM-yyyy}.");
	}

	/// <summary>
	/// Used before overwriting an existing JV: its own fiscal year must still be open.
	/// Legacy JVs without fiscalyearid are checked by their date.
	/// </summary>
	public async Task EnsureOpenAsync(JvPostingDbContext db, FiTransactionMaster existing, int entityId, CancellationToken cancellationToken)
	{
		if (existing.FiscalYearId is not int fiscalYearId)
		{
			await ResolveOpenAsync(db, existing.ClientId ?? 0, existing.EntityId ?? entityId, existing.TransactionDate, cancellationToken);
			return;
		}

		var year = await db.FiscalYears.AsNoTracking().FirstOrDefaultAsync(x => x.Id == fiscalYearId, cancellationToken);
		if (year == null || !year.Status || year.Blocked)
		{
			throw new JvPostingException(
				$"JV {existing.Code} belongs to a closed fiscal year and cannot be changed.");
		}
	}
}
