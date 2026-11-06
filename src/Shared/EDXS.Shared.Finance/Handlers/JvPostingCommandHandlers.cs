using EDXS.Shared.Finance.Commands;
using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Finance.Engine;
using EDXS.Shared.Infrastructure.CQRS;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Finance.Handlers;

public sealed class JvPostingCommandHandlers :
	ICommandHandler<PostJvCommand, JvPostingResult>,
	ICommandHandler<RepostJvCommand, JvPostingResult>,
	ICommandHandler<ReverseJvCommand, JvPostingResult>,
	ICommandHandler<PostJvBacklogCommand, IReadOnlyList<JvBacklogPostResult>>
{
	private readonly JvPostingEngine _engine;
	private readonly DbContext _caller;
	private readonly IEnumerable<IJvBacklogProvider> _backlogProviders;

	public JvPostingCommandHandlers(JvPostingEngine engine, DbContext caller, IEnumerable<IJvBacklogProvider> backlogProviders)
	{
		_engine = engine;
		_caller = caller;
		_backlogProviders = backlogProviders;
	}

	public Task<JvPostingResult> Handle(PostJvCommand request, CancellationToken cancellationToken) =>
		_engine.PostAsync(request, cancellationToken);

	public Task<JvPostingResult> Handle(RepostJvCommand request, CancellationToken cancellationToken) =>
		_engine.RepostAsync(request, cancellationToken);

	public Task<JvPostingResult> Handle(ReverseJvCommand request, CancellationToken cancellationToken) =>
		_engine.ReverseAsync(request, cancellationToken);

	public async Task<IReadOnlyList<JvBacklogPostResult>> Handle(PostJvBacklogCommand request, CancellationToken cancellationToken)
	{
		var provider = _backlogProviders.FirstOrDefault(x => string.Equals(x.Source, request.Source, StringComparison.OrdinalIgnoreCase))
			?? throw new JvPostingException($"No backlog provider is registered for '{request.Source}'.");
		var transaction = _caller.Database.CurrentTransaction
			?? throw new InvalidOperationException("Backlog posting must run inside a transaction.");

		var results = new List<JvBacklogPostResult>(request.SourceIds.Count);
		var index = 0;
		foreach (var sourceId in request.SourceIds.Distinct())
		{
			var savepoint = $"jv_backlog_{index++}";
			await transaction.CreateSavepointAsync(savepoint, cancellationToken);
			try
			{
				var posting = await provider.BuildPostingAsync(request.ClientId, request.EntityId, sourceId, cancellationToken);
				var result = await _engine.PostAsync(posting with
				{
					UserName = posting.UserName ?? request.UserName,
					IpAddress = posting.IpAddress ?? request.IpAddress,
				}, cancellationToken);
				await provider.OnPostedAsync(request.ClientId, sourceId, result, cancellationToken);
				await transaction.ReleaseSavepointAsync(savepoint, cancellationToken);
				results.Add(new JvBacklogPostResult(sourceId, true, result, null));
			}
			catch (InvalidOperationException ex)
			{
				await transaction.RollbackToSavepointAsync(savepoint, cancellationToken);
				_caller.ChangeTracker.Clear();
				results.Add(new JvBacklogPostResult(sourceId, false, null, ex.Message));
			}
		}

		return results;
	}
}
