using HumanResource.Service.PayrollAccounting;
using HumanResource.Service.PayrollAccounting.Commands;
using HumanResource.Service.PayrollAccounting.Contracts;
using HumanResource.Service.PayrollAccounting.Data;
using HumanResource.Service.PayrollAccounting.Engine;
using FluentAssertions;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Handlers.Commands;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Posting;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanResource.Service.Tests;

/// <summary>Pay slip save -> PayrollProcessedEvent -> payroll JV, on SQLite with the real handler.</summary>
public sealed class PayrollJvPostingTests : IAsyncLifetime
{
    private const int ClientId = 1, EntityId = 1;
    private const int SalaryExpense = 501, SalaryPayable = 502;
    private const int PostingDept = 10, UnsetDept = 11, HalfSetDept = 12;
    private const int PeriodDetailId = 7, RegisterId = 3;

    private static readonly JvPostingContextFactory Factory = new((options, connection) => options.UseSqlite(connection));

    private SqliteConnection _connection = null!;
    private HumanResourceDbContext _db = null!;
    private ServiceProvider _services = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();
        _db = new HumanResourceDbContext(new DbContextOptionsBuilder<HumanResourceDbContext>().UseSqlite(_connection).Options);
        await _db.Database.EnsureCreatedAsync();
        await CreatePostingTablesAsync();
        await SeedAsync();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_db);
        services.AddSingleton<DbContext>(_db);
        services.AddSingleton<IJvPostingContextFactory>(Factory);
        services.AddSingleton<IFiPostingLock, NoopLock>();
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(typeof(PayrollJvBuilder).Assembly, typeof(PostJvCommand).Assembly));
        services.AddJvPosting();
        services.AddScoped<PayrollJvBuilder>();
        services.AddScoped<PayrollJvBacklogProvider>();
        _services = services.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await _services.DisposeAsync();
        await _db.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private PaySlipV3CommandHandlers Handler => new(_db, _services.GetRequiredService<IPublisher>());

    private static SavePaySlipProcessCommand PaySlip(int dept, double gross, int id = 0) => new(new PaySlipV3Dto
    {
        Id = id,
        perioddtlid = PeriodDetailId,
        registerid = RegisterId,
        deptId = dept,
        grosssalary = gross,
    }, ClientId, EntityId);

    [Fact]
    public async Task ProcessedPaySlip_PostsDepartmentJv()
    {
        var saved = await Handler.Handle(PaySlip(PostingDept, 85000), default);

        var jv = (await JvsAsync()).Should().ContainSingle().Subject;
        jv.ReferenceNo.Should().Be($"PAYR:{saved.Id}");
        jv.TransactionDate.Should().Be(new DateTime(2026, 9, 30));
        jv.Details.Should().ContainSingle(x => x.AccountId == SalaryExpense && x.Debit == 85000);
        jv.Details.Should().ContainSingle(x => x.AccountId == SalaryPayable && x.Credit == 85000);
    }

    [Fact]
    public async Task ReprocessedPaySlip_OverwritesSameJv()
    {
        var saved = await Handler.Handle(PaySlip(PostingDept, 85000), default);
        var first = (await JvsAsync()).Single();
        _db.ChangeTracker.Clear();

        await Handler.Handle(PaySlip(PostingDept, 90000, saved.Id), default);

        var jv = (await JvsAsync()).Should().ContainSingle().Subject;
        jv.Id.Should().Be(first.Id);
        jv.RecordVersion.Should().Be(2);
        jv.Details.Should().ContainSingle(x => x.AccountId == SalaryExpense && x.Debit == 90000);
    }

    [Fact]
    public async Task DepartmentWithoutAccounts_SavesPaySlipWithoutJv()
    {
        var saved = await Handler.Handle(PaySlip(UnsetDept, 50000), default);

        saved.Id.Should().BeGreaterThan(0);
        (await JvsAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task DepartmentWithOneAccount_FailsClearlyAndSavesNothing()
    {
        var act = () => Handler.Handle(PaySlip(HalfSetDept, 50000), default);

        await act.Should().ThrowAsync<JvPostingException>().WithMessage("*Admin*needs both payroll accounts*");
        _db.ChangeTracker.Clear();
        (await _db.PayrollProcesses.AsNoTracking().AnyAsync()).Should().BeFalse("the pay slip rolls back with its JV");
    }

    [Fact]
    public async Task Backlog_ListsPaySlipsOfPostingDepartmentsWithoutJv()
    {
        // Saved without the publisher: as if processed before JV posting existed.
        await new PaySlipV3CommandHandlers(_db).Handle(PaySlip(PostingDept, 70000), default);
        await new PaySlipV3CommandHandlers(_db).Handle(PaySlip(UnsetDept, 10000), default);
        var provider = _services.GetRequiredService<PayrollJvBacklogProvider>();

        var backlog = await provider.GetBacklogAsync(ClientId, EntityId, null, null, default);

        backlog.Should().ContainSingle().Which.Amount.Should().Be(70000);
        var engine = _services.GetRequiredService<JvPostingEngine>();
        await FinanceTransaction.ExecuteAsync(_db, async ct =>
            await engine.PostAsync(await provider.BuildPostingAsync(ClientId, EntityId, backlog[0].SourceId, ct), ct), default);
        (await provider.GetBacklogAsync(ClientId, EntityId, null, null, default)).Should().BeEmpty();
    }

    [Fact]
    public async Task Reconciliation_CountsOnlyPostingDepartments()
    {
        await Handler.Handle(PaySlip(PostingDept, 60000), default);
        await new PaySlipV3CommandHandlers(_db).Handle(PaySlip(PostingDept, 40000), default);
        await Handler.Handle(PaySlip(UnsetDept, 99999), default);

        var line = await _services.GetRequiredService<PayrollJvBacklogProvider>().ReconcileAsync(ClientId, EntityId, null, null, default);

        line.DocumentCount.Should().Be(2);
        line.DocumentTotal.Should().Be(100000);
        line.JvTotal.Should().Be(60000);
        line.Difference.Should().Be(40000);
        line.BacklogCount.Should().Be(1);
    }

    private async Task<List<FiTransactionMaster>> JvsAsync()
    {
        await using var jv = Factory.Create(_db);
        return await jv.TransactionMasters.AsNoTracking().Include(x => x.Details.Where(d => !d.Blocked)).OrderBy(x => x.Id).ToListAsync();
    }

    private async Task CreatePostingTablesAsync()
    {
        await using var jv = Factory.Create(_db);
        foreach (var statement in jv.Database.GenerateCreateScript().Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var marker = statement.IndexOf("CREATE TABLE \"", StringComparison.Ordinal);
            if (marker < 0) continue;
            var start = marker + "CREATE TABLE \"".Length;
            var table = statement[start..statement.IndexOf('"', start)];
            var exists = await _db.Database.SqlQuery<int>($"SELECT COUNT(*) AS Value FROM sqlite_master WHERE type = 'table' AND name = {table} COLLATE NOCASE").SingleAsync();
            if (exists == 0) await _db.Database.ExecuteSqlRawAsync(statement);
        }
        await _db.Database.ExecuteSqlRawAsync("ALTER TABLE OrgDepartment ADD COLUMN ficoaid INTEGER");
        await _db.Database.ExecuteSqlRawAsync("ALTER TABLE OrgDepartment ADD COLUMN ficoaid_rec INTEGER");
    }

    private async Task SeedAsync()
    {
        await using (var jv = Factory.Create(_db))
        {
            jv.Accounts.AddRange(
                new FiChartOfAccount { Id = SalaryExpense, Title = "Salary Expense", Subsidiary = true, Active = true, ClientId = ClientId },
                new FiChartOfAccount { Id = SalaryPayable, Title = "Salary Payable", Subsidiary = true, Active = true, ClientId = ClientId });
            jv.FiscalYears.Add(new FiFiscalYear
            {
                Id = 1, Code = "FY26", Title = "FY 2026", Status = true, ClientId = ClientId, EntityId = EntityId,
                StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31),
            });
            await jv.SaveChangesAsync();
        }

        await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO OrgDepartment (Id, depcode, depstxt, clientid, ficoaid, ficoaid_rec) VALUES ({PostingDept}, 'ACD', 'Academics', {ClientId}, {SalaryExpense}, {SalaryPayable})");
        await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO OrgDepartment (Id, depcode, depstxt, clientid) VALUES ({UnsetDept}, 'TRN', 'Transport', {ClientId})");
        await _db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO OrgDepartment (Id, depcode, depstxt, clientid, ficoaid) VALUES ({HalfSetDept}, 'ADM', 'Admin', {ClientId}, {SalaryExpense})");

        _db.PayrollPeriods.Add(new ETPayperiodhdrLegacy
        {
            Id = 1, Code = "P26", ClientId = ClientId, EntityId = EntityId, Payrollregisterid = RegisterId,
            Details = [new ETPayperioddtlLegacy { Id = PeriodDetailId, Year = 2026, Monthid = 9, Startdate = new DateTime(2026, 9, 1), Enddate = new DateTime(2026, 9, 30) }],
        });
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
    }

    private sealed class NoopLock : IFiPostingLock
    {
        public Task AcquireAsync(DbContext db, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
