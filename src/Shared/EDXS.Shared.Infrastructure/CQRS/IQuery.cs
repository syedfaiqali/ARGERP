using MediatR;

namespace EDXS.Shared.Infrastructure.CQRS;

/// <summary>
/// Marker interface for queries that retrieve data without modifying state
/// </summary>
/// <typeparam name="TResponse">The response type</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Handler for queries
/// </summary>
/// <typeparam name="TQuery">The query type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
	where TQuery : IQuery<TResponse>
{
}
