using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Infrastructure.CQRS;

namespace EDXS.Shared.Finance.Commands;

/// <summary>
/// Document edited: overwrites the existing JV in place (same Id and code, lines replaced).
/// The JV is located by Source/SourceId, or by <paramref name="TransactionId"/> for a manual JV.
/// When a source has no active JV yet, a new one is posted.
/// Blocked when the existing JV belongs to a closed fiscal year.
/// </summary>
public sealed record RepostJvCommand(PostJvCommand NewPosting, int? TransactionId = null) : ICommand<JvPostingResult>;
