using HumanResource.Service.PayrollAccounting.Commands;
using HumanResource.Service.PayrollAccounting.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.PayrollAccounting.Api;

public sealed record PostJvBacklogRequest(string Source, IReadOnlyList<string> SourceIds);

/// <summary>
/// Backlog and reconciliation endpoints. The backlog providers live in the service that owns the
/// documents, so each such service exposes a derived controller under its own route:
///   GET  {route}?source=&amp;from=&amp;to=     documents that should have a JV but do not
///   POST {route}/post                     post the selected documents
///   GET  {route}/reconciliation?from=&amp;to=  document totals vs JV totals per source
/// </summary>
[ApiController]
[Authorize]
public abstract class JvBacklogControllerBase : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly DbContext _db;

	protected JvBacklogControllerBase(IMediator mediator, DbContext db)
	{
		_mediator = mediator;
		_db = db;
	}

	private int HeaderInt(string name) =>
		Request.Headers.TryGetValue(name, out var value) && int.TryParse(value.ToString(), out var number) ? number : 0;

	private int ClientId => HeaderInt("X-Client-Id");

	private int EntityId(int? entityId) => entityId ?? HeaderInt("X-Entity-Id");

	[HttpGet]
	public async Task<IActionResult> GetBacklog([FromQuery] string? source, [FromQuery] DateTime? from, [FromQuery] DateTime? to,
		[FromQuery] int? entityId, CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new GetJvBacklogQuery(ClientId, EntityId(entityId), source, from, to), cancellationToken);
		return Ok(new { IsSuccessCall = true, Data = result });
	}

	[HttpGet("reconciliation")]
	public async Task<IActionResult> GetReconciliation([FromQuery] DateTime? from, [FromQuery] DateTime? to,
		[FromQuery] int? entityId, CancellationToken cancellationToken)
	{
		var result = await _mediator.Send(new GetJvReconciliationQuery(ClientId, EntityId(entityId), from, to), cancellationToken);
		return Ok(new { IsSuccessCall = true, Data = result });
	}

	[HttpPost("post")]
	public async Task<IActionResult> Post([FromBody] PostJvBacklogRequest request, [FromQuery] int? entityId, CancellationToken cancellationToken)
	{
		if (request.SourceIds == null || request.SourceIds.Count == 0)
			return BadRequest(new { IsSuccessCall = false, Errors = new[] { new { Description = "Select document(s) to post." } } });

		// Each document gets its own savepoint inside this transaction.
		var command = new PostJvBacklogCommand(ClientId, EntityId(entityId), request.Source, request.SourceIds);
		var result = await FinanceTransaction.ExecuteAsync(_db, ct => _mediator.Send(command, ct), cancellationToken);
		return Ok(new { IsSuccessCall = true, Data = result });
	}
}
