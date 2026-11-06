using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Infrastructure.CQRS.Behaviors;

/// <summary>
/// Pipeline behavior that wraps command execution in a database transaction
/// Only applies to commands (not queries)
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly DbContext? _dbContext;

	public TransactionBehavior(DbContext? dbContext = null)
	{
		_dbContext = dbContext;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		// Only apply transactions to commands
		if (request is not ICommand and not ICommand<TResponse>)
		{
			return await next();
		}

		// If no DbContext available, skip transaction
		if (_dbContext == null)
		{
			return await next();
		}

		// If already in a transaction, don't create nested transaction
		if (_dbContext.Database.CurrentTransaction != null)
		{
			return await next();
		}

		await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
		
		try
		{
			var response = await next();
			await transaction.CommitAsync(cancellationToken);
			return response;
		}
		catch
		{
			await transaction.RollbackAsync(cancellationToken);
			throw;
		}
	}
}
