using EDXS.Shared.Finance.Commands;
using EDXS.Shared.Finance.Contracts;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Posting
{
    /// <summary>
    /// Payroll JV, one per processed pay slip (PAYR:&lt;process id&gt;):
    ///   Dr OrgDepartment.ficoaid       gross salary
    ///   Cr OrgDepartment.ficoaid_rec   gross salary
    /// A department without either account is not set up for posting and is skipped;
    /// a department with only one of them is a setup error.
    /// </summary>
    public sealed class PayrollJvBuilder
    {
        private readonly HumanResourceDbContext _db;

        public PayrollJvBuilder(HumanResourceDbContext db)
        {
            _db = db;
        }

        // The accounts are read directly so the HR model (and its migrations) stay unchanged.
        public sealed record DepartmentAccounts(int Id, int? DebitAccountId, int? CreditAccountId, string? Name)
        {
            public bool IsConfigured => DebitAccountId.HasValue || CreditAccountId.HasValue;
        }

        public Task<HRpayrollprocessLegacy?> LoadProcessAsync(int processId, int clientId, CancellationToken cancellationToken) =>
            _db.PayrollProcesses.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == processId && x.clientid == clientId, cancellationToken);

        public Task<DepartmentAccounts?> LoadDepartmentAsync(int departmentId, CancellationToken cancellationToken) =>
            _db.Database
                .SqlQuery<DepartmentAccounts>($"SELECT Id, ficoaid AS DebitAccountId, ficoaid_rec AS CreditAccountId, depstxt AS Name FROM OrgDepartment WHERE Id = {departmentId}")
                .FirstOrDefaultAsync(cancellationToken);

        public Task<List<int>> PostingDepartmentIdsAsync(int clientId, CancellationToken cancellationToken) =>
            _db.Database
                .SqlQuery<int>($"SELECT Id AS Value FROM OrgDepartment WHERE clientid = {clientId} AND ficoaid IS NOT NULL AND ficoaid_rec IS NOT NULL")
                .ToListAsync(cancellationToken);

        /// <summary>The JV of one pay slip, or null when there is nothing to post or the department is not set up.</summary>
        public async Task<PostJvCommand?> BuildAsync(HRpayrollprocessLegacy process, CancellationToken cancellationToken)
        {
            var gross = Math.Round((decimal)(process.grosssalary ?? 0), 2);
            if (process.utblock || gross <= 0 || process.deptid is not int departmentId) return null;

            var department = await LoadDepartmentAsync(departmentId, cancellationToken);
            if (department == null || !department.IsConfigured) return null;
            if (!department.DebitAccountId.HasValue || !department.CreditAccountId.HasValue)
            {
                throw new JvPostingException(
                    $"Department '{department.Name ?? departmentId.ToString()}' needs both payroll accounts (salary expense and salary payable) to post the payroll JV.");
            }

            return new PostJvCommand
            {
                ClientId = process.clientid ?? 0,
                EntityId = process.entityid ?? 0,
                Source = JvSources.Payroll,
                SourceId = process.Id.ToString(),
                TransactionDate = (process.tilldate ?? process.transdate ?? DateTime.Today).Date,
                Narration = $"Payroll {process.transno} ({department.Name})",
                Lines =
                [
                    JvLine.Dr(department.DebitAccountId.Value, gross, "Gross salary"),
                    JvLine.Cr(department.CreditAccountId.Value, gross, "Salary payable"),
                ],
            };
        }
    }
}
