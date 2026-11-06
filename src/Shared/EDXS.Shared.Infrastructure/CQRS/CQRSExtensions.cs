using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using EDXS.Shared.Infrastructure.CQRS.Behaviors;

namespace EDXS.Shared.Infrastructure.CQRS;

/// <summary>
/// Extension methods for registering CQRS infrastructure
/// </summary>
public static class CQRSExtensions
{
	/// <summary>
	/// Adds MediatR with CQRS pipeline behaviors
	/// </summary>
	/// <param name="services">Service collection</param>
	/// <param name="assemblies">Assemblies to scan for handlers and validators</param>
	/// <returns>Service collection for chaining</returns>
	public static IServiceCollection AddCQRS(this IServiceCollection services, params Assembly[] assemblies)
	{
		// Register MediatR
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblies(assemblies);

			// Add pipeline behaviors in order: Logging -> Validation -> Transaction
			cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
			cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
			cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
		});

		// Register FluentValidation validators
		foreach (var assembly in assemblies)
		{
			services.AddValidatorsFromAssembly(assembly);
		}

		return services;
	}

	/// <summary>
	/// Adds MediatR with CQRS pipeline behaviors (scans calling assembly)
	/// </summary>
	/// <param name="services">Service collection</param>
	/// <returns>Service collection for chaining</returns>
	public static IServiceCollection AddCQRS(this IServiceCollection services)
	{
		var callingAssembly = Assembly.GetCallingAssembly();
		return services.AddCQRS(callingAssembly);
	}
}
