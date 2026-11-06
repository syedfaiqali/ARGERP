using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Infrastructure.CQRS;

namespace EDXS.Shared.Finance.Queries;

/// <summary>The active JV of a source document, or null.</summary>
public sealed record GetJvBySourceQuery(int ClientId, string Source, string SourceId) : IQuery<JvTrailItem?>;

public sealed record GetJvPostingStatusQuery(int ClientId, string Source, string SourceId) : IQuery<JvPostingStatus>;

/// <summary>Every JV (posts and reversals) written for a source document, oldest first.</summary>
public sealed record GetJvTrailQuery(int ClientId, string Source, string SourceId) : IQuery<IReadOnlyList<JvTrailItem>>;

/// <summary>Unposted documents from the registered backlog providers; Source null means all.</summary>
public sealed record GetJvBacklogQuery(int ClientId, int EntityId, string? Source, DateTime? From, DateTime? To)
	: IQuery<IReadOnlyList<JvBacklogItem>>;

/// <summary>Document totals vs JV totals per registered source (the service's backlog providers).</summary>
public sealed record GetJvReconciliationQuery(int ClientId, int EntityId, DateTime? From, DateTime? To)
	: IQuery<IReadOnlyList<JvReconciliationLine>>;

/// <summary>Back-link: the source document of a JV, or null when the JV does not exist.</summary>
public sealed record GetJvSourceQuery(int ClientId, int TransactionId) : IQuery<JvSourceInfo?>;
