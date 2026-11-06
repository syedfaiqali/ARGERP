using FluentValidation;
using MediatR;

namespace EDXS.Shared.Infrastructure.CQRS.Behaviors;

/// <summary>
/// Pipeline behavior that validates commands/queries before execution using FluentValidation
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (!_validators.Any())
		{
			return await next();
		}

		var context = new ValidationContext<TRequest>(request);

		var validationResults = await Task.WhenAll(
			_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

		var failures = validationResults
			.SelectMany(r => r.Errors)
			.Where(f => f != null)
			.ToList();

		if (failures.Count != 0)
		{
			throw new ValidationException(failures);
		}

		return await next();
	}
}
