using MediatR;

namespace EDXS.Shared.Infrastructure.CQRS;

/// <summary>
/// Marker interface for commands that modify state and return a result
/// </summary>
/// <typeparam name="TResponse">The response type</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Marker interface for commands that modify state without returning a value
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Handler for commands that return a result
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
	where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Handler for commands that don't return a value
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
	where TCommand : ICommand
{
}
