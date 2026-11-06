using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HumanResource.Service.PayrollAccounting.Data;

public interface IJvPostingContextFactory
{
	/// <summary>
	/// Creates a <see cref="JvPostingDbContext"/> on the caller's connection, enlisted in the caller's
	/// current transaction (if any). The caller keeps ownership of the connection; dispose the result.
	/// </summary>
	JvPostingDbContext Create(DbContext caller);
}

public sealed class JvPostingContextFactory : IJvPostingContextFactory
{
	private readonly Action<DbContextOptionsBuilder<JvPostingDbContext>, DbConnection> _configure;

	/// <param name="configure">Provider setup, e.g. (o, c) => o.UseSqlServer(c)</param>
	public JvPostingContextFactory(Action<DbContextOptionsBuilder<JvPostingDbContext>, DbConnection> configure)
	{
		_configure = configure;
	}

	public JvPostingDbContext Create(DbContext caller)
	{
		var builder = new DbContextOptionsBuilder<JvPostingDbContext>();
		_configure(builder, caller.Database.GetDbConnection());

		var context = new JvPostingDbContext(builder.Options);
		var transaction = caller.Database.CurrentTransaction;
		if (transaction != null)
		{
			context.Database.UseTransaction(transaction.GetDbTransaction());
		}

		return context;
	}
}
