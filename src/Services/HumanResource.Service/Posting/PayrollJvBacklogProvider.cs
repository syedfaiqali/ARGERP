using EDXS.Shared.Finance.Commands;
using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Finance.Data;
using EDXS.Shared.Finance.Engine;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Posting
{
    /// <summary>Pay slips of departments set up for posting that have no active payroll JV.</summary>
    public sealed class PayrollJvBacklogProvider : IJvBacklogProvider
    {
        private const int MaxItems = 2000;

        private readonly HumanResourceDbContext _db;
        private readonly PayrollJvBuilder _builder;
        private readonly IJvPostingContextFactory _contextFactory;

        public PayrollJvBacklogProvider(HumanResourceDbContext db, PayrollJvBuilder builder, IJvPostingContextFactory contextFactory)
        {
            _db = db;
            _builder = builder;
            _contextFactory = contextFactory;
        }

        public string Source => JvSources.Payroll;

        private IQueryable<Entities.HRpayrollprocessLegacy> Postable(int clientId, int entityId, List<int> departments, DateTime? from, DateTime? to)
        {
            var query = _db.PayrollProcesses.AsNoTracking()
                .Where(x => x.clientid == clientId && x.entityid == entityId && !x.utblock
                    && x.grosssalary > 0 && x.deptid != null && departments.Contains(x.deptid.Value));
            if (from.HasValue) query = query.Where(x => x.tilldate >= from.Value.Date);
            if (to.HasValue) query = query.Where(x => x.tilldate < to.Value.Date.AddDays(1));
            return query;
        }

        public async Task<JvReconciliationLine> ReconcileAsync(int clientId, int entityId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var label = JvSources.Find(Source)!.Label;
            var departments = await _builder.PostingDepartmentIdsAsync(clientId, cancellationToken);
            if (departments.Count == 0) return new JvReconciliationLine(Source, label, 0, 0, 0, 0);

            var documents = await Postable(clientId, entityId, departments, from, to)
                .Select(x => new { x.Id, x.grosssalary })
                .ToListAsync(cancellationToken);

            await using var jv = _contextFactory.Create(_db);
            var own = await JvReference.ActiveAmountsAsync(jv, clientId,
                documents.Select(x => JvReference.Build(Source, x.Id.ToString())).ToList(), cancellationToken);

            var documentTotal = documents.Sum(x => Math.Round((decimal)(x.grosssalary ?? 0), 2));
            var jvTotal = documents.Sum(x => own.GetValueOrDefault(JvReference.Build(Source, x.Id.ToString())));
            var backlog = documents.Count(x => !own.ContainsKey(JvReference.Build(Source, x.Id.ToString())));
            return new JvReconciliationLine(Source, label, documents.Count, documentTotal, jvTotal, backlog);
        }

        public async Task<IReadOnlyList<JvBacklogItem>> GetBacklogAsync(int clientId, int entityId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var departments = await _builder.PostingDepartmentIdsAsync(clientId, cancellationToken);
            if (departments.Count == 0) return [];

            var query = Postable(clientId, entityId, departments, from, to);

            var candidates = await query
                .OrderBy(x => x.tilldate).ThenBy(x => x.Id)
                .Select(x => new { x.Id, x.transno, x.tilldate, x.transdate, x.grosssalary })
                .Take(MaxItems)
                .ToListAsync(cancellationToken);
            if (candidates.Count == 0) return [];

            await using var jv = _contextFactory.Create(_db);
            var active = await JvReference.FindActiveAsync(jv, clientId,
                candidates.Select(x => JvReference.Build(Source, x.Id.ToString())).ToList(), cancellationToken);

            return candidates
                .Where(x => !active.Contains(JvReference.Build(Source, x.Id.ToString())))
                .Select(x => new JvBacklogItem(Source, x.Id.ToString(), x.transno, x.tilldate ?? x.transdate ?? DateTime.MinValue,
                    (decimal)(x.grosssalary ?? 0), "Processed before payroll JV posting"))
                .ToList();
        }

        public async Task<PostJvCommand> BuildPostingAsync(int clientId, int entityId, string sourceId, CancellationToken cancellationToken)
        {
            var process = int.TryParse(sourceId, out var id) ? await _builder.LoadProcessAsync(id, clientId, cancellationToken) : null;
            if (process == null || process.utblock) throw new JvPostingException($"Pay slip {sourceId} was not found.");

            return await _builder.BuildAsync(process, cancellationToken)
                ?? throw new JvPostingException($"Pay slip {process.transno} has no amount or its department is not set up for posting.");
        }

        public Task OnPostedAsync(int clientId, string sourceId, JvPostingResult result, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
