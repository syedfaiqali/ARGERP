using EDXS.Shared.Workflow;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Workflow;

public sealed class EarningTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateEarningTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.EarningType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetEarningTypeByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateEarningTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateEarningTypeV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateEarningTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateEarningTypeV3Command(new(id, r.Code, r.EarningType, r.Description, r.BaseSalary, r.PayslipIndex, r.PayslipDescription,
            r.UomId, r.PaymentRunTypeId, r.TaxMethodId, r.RateBasisId, r.BasisEarningTypeId, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteEarningTypeV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateEarningTypeV3Request r) => string.IsNullOrWhiteSpace(r.Code) ? r.EarningType : $"{r.Code} · {r.EarningType}";
}

public sealed class EarningGroupWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateEarningGroupV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.EarningGroup;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetEarningGroupByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateEarningGroupV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateEarningGroupV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateEarningGroupV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateEarningGroupV3Command(new(id, r.Code, r.EarningGroup, r.IsActive, r.Details), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteEarningGroupV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateEarningGroupV3Request r) => string.IsNullOrWhiteSpace(r.Code) ? r.EarningGroup : $"{r.Code} · {r.EarningGroup}";
}

public sealed class DeductionTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateDeductionTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.DeductionType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetDeductionTypeByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateDeductionTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateDeductionTypeV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateDeductionTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateDeductionTypeV3Command(new(id, r.Code, r.DeductionType, r.Description, r.PaymentRunTypeId, r.Reimbursed, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteDeductionTypeV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateDeductionTypeV3Request r) => string.IsNullOrWhiteSpace(r.Code) ? r.DeductionType : $"{r.Code} · {r.DeductionType}";
}

public sealed class DeductionGroupWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateDeductionGroupV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.DeductionGroup;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetDeductionGroupByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateDeductionGroupV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateDeductionGroupV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateDeductionGroupV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateDeductionGroupV3Command(new(id, r.Code, r.DeductionGroup, r.IsActive, r.Details), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteDeductionGroupV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateDeductionGroupV3Request r) => string.IsNullOrWhiteSpace(r.Code) ? r.DeductionGroup : $"{r.Code} · {r.DeductionGroup}";
}

/// <summary>
/// Payroll period handlers find records by id alone, with no tenant check. This adapter confirms the
/// tenant itself before snapshotting, updating or deleting, so a request can never reach another
/// tenant's period. The update request is the payload because it is a superset of the create request.
/// </summary>
public sealed class PayrollPeriodWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<UpdatePayrollPeriodV3Request>(db)
{
    private HumanResourceDbContext Context => (HumanResourceDbContext)Database;
    public override ActionRegistration Registration => PilotActions.PayrollPeriod;

    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await Context.PayrollPeriods.AsNoTracking()
            .Where(x => x.Id == id && x.ClientId == clientId && x.Utblock != true)
            .Select(x => new
            {
                x.Id, x.Code, x.Date, x.Atntypeid, x.Fromyear, x.Toyear, x.Ltxt, x.Startfrom, x.Statusid, x.Lovsharedid,
                x.Startdate, x.Payrollregisterid, x.Noofperiod,
                Details = x.Details.OrderBy(d => d.Lineindex)
                    .Select(d => new { d.Id, d.Periodstxt, d.Year, d.Startdate, d.Enddate, d.Noofdays, d.Disbdate, d.Statusid, d.Lineindex }).ToList()
            })
            .FirstOrDefaultAsync(ct);

    protected override async Task<(object Row, int Id)> CreateAsync(UpdatePayrollPeriodV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var details = (r.Details ?? Enumerable.Empty<UpdatePayrollPeriodDetailV3Request>())
            .Select(d => new CreatePayrollPeriodDetailV3Request(d.Periodstxt, d.Year, d.StartDate, d.EndDate, d.NoOfDays, d.DisbDate, d.StatusId, d.LineIndex))
            .ToList();
        var create = new CreatePayrollPeriodV3Request(r.Code, r.Date, r.Atntypeid, r.Fromyear, r.Toyear, r.Ltxt, r.Startfrom, r.Statusid,
            r.Lovsharedid, r.Startdate, r.Payrollregisterid, r.Noofperiod, r.IsActive, details);
        var id = await mediator.Send(new CreatePayrollPeriodV3Command(clientId, entityId, create), ct);
        // The screen's controller answered a create with the bare id; keep that response shape.
        return (id, id);
    }

    protected override async Task<bool> UpdateAsync(int id, UpdatePayrollPeriodV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        await OwnedAsync(id, clientId, ct) && await mediator.Send(new UpdatePayrollPeriodV3Command(clientId, entityId, r with { Id = id }), ct);

    protected override async Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await OwnedAsync(id, clientId, ct) && await mediator.Send(new DeletePayrollPeriodV3Command(id), ct);

    private Task<bool> OwnedAsync(int id, int clientId, CancellationToken ct) =>
        Context.PayrollPeriods.AnyAsync(x => x.Id == id && x.ClientId == clientId && x.Utblock != true, ct);

    protected override string Subject(UpdatePayrollPeriodV3Request r) =>
        string.IsNullOrWhiteSpace(r.Ltxt) ? $"Payroll Period {r.Code}" : $"{r.Code} · {r.Ltxt}";
}
