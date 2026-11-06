using HumanResource.Service.PayrollAccounting.Contracts;
using HumanResource.Service.PayrollAccounting.Data;
using HumanResource.Service.PayrollAccounting.Engine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HumanResource.Service.PayrollAccounting;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers the JV posting engine. The host must also:
	///  - register its DbContext as the base <see cref="DbContext"/> (the engine and TransactionBehavior share it)
	///  - include typeof(PostJvCommand).Assembly in AddCQRS(...) so the handlers and validators are found
	/// </summary>
	public static IServiceCollection AddJvPosting(this IServiceCollection services)
	{
		services.TryAddSingleton(TimeProvider.System);
		services.AddHttpContextAccessor();
		services.TryAddScoped<IJvPostingUserContext, HttpJvPostingUserContext>();
		services.TryAddSingleton<IJvPostingContextFactory>(
			new JvPostingContextFactory((options, connection) => options.UseSqlServer(connection, SqlServerOptions)));
		services.TryAddSingleton<IFiPostingLock, SqlServerFiPostingLock>();
		services.TryAddSingleton<FiscalYearResolver>();
		services.TryAddSingleton<PostingPolicyValidator>();
		services.TryAddScoped<JvPostingEngine>();
		return services;
	}

	/// <summary>
	/// Same SQL Server options as the services: list Contains() must not become OPENJSON ... WITH,
	/// which older database compatibility levels reject.
	/// </summary>
	public static void SqlServerOptions(Microsoft.EntityFrameworkCore.Infrastructure.SqlServerDbContextOptionsBuilder sql) =>
		sql.TranslateParameterizedCollectionsToConstants();
}
