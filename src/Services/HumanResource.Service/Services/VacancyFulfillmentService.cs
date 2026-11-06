using System.Data;
using System.Text.Json;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Services;

public interface IVacancyFulfillmentService
{
    Task<HRVacancyPosition> ValidateAvailablePositionAsync(int positionId, int clientId, int entityId, CancellationToken cancellationToken);
    Task FillFromNewEmployeeAsync(int positionId, int employeeId, int clientId, int entityId, string actor, CancellationToken cancellationToken);
    Task<HRVacancyPositionAssignment> SchedulePromotionAsync(int positionId, int employeeId, DateTime effectiveDate, string? remarks, int clientId, int entityId, string actor, CancellationToken cancellationToken);
    Task ActivateAssignmentAsync(int assignmentId, string actor, CancellationToken cancellationToken);
    Task<int> ReleaseEmployeePositionsAsync(int employeeId, int clientId, int entityId, DateTime effectiveDate, string actor, string reason, CancellationToken cancellationToken);
    Task<int> ReleaseDueEmployeeExitsAsync(CancellationToken cancellationToken);
    Task<int> ActivateDuePromotionsAsync(CancellationToken cancellationToken);
}

public sealed class VacancyFulfillmentService : IVacancyFulfillmentService
{
    private static readonly DateTime FarFuture = new(9998, 12, 31);
    private readonly HumanResourceDbContext _context;
    private readonly ILogger<VacancyFulfillmentService> _logger;

    public VacancyFulfillmentService(HumanResourceDbContext context, ILogger<VacancyFulfillmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HRVacancyPosition> ValidateAvailablePositionAsync(
        int positionId,
        int clientId,
        int entityId,
        CancellationToken cancellationToken)
    {
        var position = await _context.VacancyPositions
            .Join(_context.Vacancies, p => p.VacancyId, v => v.Id, (p, v) => new { Position = p, Vacancy = v })
            .Where(x => x.Position.Id == positionId
                && x.Vacancy.ClientId == clientId
                && x.Vacancy.EntityId == entityId
                && !x.Vacancy.IsDeleted
                && x.Vacancy.ApprovalStatus == "Approved"
                && x.Position.Status == "Vacant")
            .Select(x => x.Position)
            .FirstOrDefaultAsync(cancellationToken);
        return position ?? throw new InvalidOperationException("The selected position is not an approved vacant position in the current scope.");
    }

    public async Task FillFromNewEmployeeAsync(
        int positionId,
        int employeeId,
        int clientId,
        int entityId,
        string actor,
        CancellationToken cancellationToken)
    {
        var position = await ValidateAvailablePositionAsync(positionId, clientId, entityId, cancellationToken);
        var activeStatusId = await GetLovStatusIdAsync("Active", cancellationToken);
        var effectiveDate = await _context.EmployeeProfiles.AsNoTracking()
            .Where(x => x.Id == employeeId)
            .Select(x => x.JoiningDate)
            .FirstOrDefaultAsync(cancellationToken) ?? DateTime.UtcNow.Date;
        if (await _context.PositionAssignments.AnyAsync(x => x.PositionId == position.PositionId
            && x.LovStatusId == activeStatusId && x.EffectiveTo == null, cancellationToken))
            throw new InvalidOperationException("The canonical position already has an active occupant.");
        if (await _context.PositionAssignments.AnyAsync(x => x.EmployeeId == employeeId
            && x.LovStatusId == activeStatusId && x.EffectiveTo == null, cancellationToken))
            throw new InvalidOperationException("The employee already occupies another active position.");
        var updatedAt = DateTime.UtcNow;
        position.Status = "Filled";
        position.CurrentEmployeeId = employeeId;
        position.NeedsReview = false;
        position.ReviewReason = null;
        position.UpdatedAtUtc = updatedAt;
        position.UpdatedBy = actor;
        VacancyUtMetadata.Touch(position, actor, updatedAt);

        var assignment = new HRVacancyPositionAssignment
        {
            PositionId = position.Id,
            EmployeeId = employeeId,
            AssignmentType = "ExternalJoin",
            Status = "Active",
            EffectiveDate = effectiveDate.Date,
            CreatedBy = actor,
            CreatedAtUtc = updatedAt,
            ActivatedAtUtc = updatedAt
        };
        VacancyUtMetadata.Initialize(assignment, actor, systemGenerated: true, timestamp: updatedAt);
        _context.VacancyPositionAssignments.Add(assignment);
        _context.PositionAssignments.Add(NewCanonicalAssignment(position.PositionId, employeeId, "ExternalJoin",
            effectiveDate.Date, activeStatusId, actor));
        AddPositionAudit(position.PositionId, clientId, entityId, "PositionOccupied", actor, null,
            new { EmployeeId = employeeId, Source = "ExternalJoin" });
        AddAudit(position.VacancyId, position.Id, clientId, entityId, "PositionFilledByNewEmployee", actor, null,
            new { position.PositionNumber, EmployeeId = employeeId });
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<HRVacancyPositionAssignment> SchedulePromotionAsync(
        int positionId,
        int employeeId,
        DateTime effectiveDate,
        string? remarks,
        int clientId,
        int entityId,
        string actor,
        CancellationToken cancellationToken)
    {
        var position = await ValidateAvailablePositionAsync(positionId, clientId, entityId, cancellationToken);
        var employee = await _context.EmployeeProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == employeeId && x.ClientId == clientId && x.EntityId == entityId
                && !(x.UtBlock ?? false) && x.StatusId != 126 && x.StatusId != 127
                && (!x.ResignDate.HasValue || x.ResignDate.Value.Date >= DateTime.UtcNow.Date), cancellationToken)
            ?? throw new InvalidOperationException("The selected active employee is outside the current scope.");

        // A vacancy may have several numbered openings, but one employee may fill
        // only one of them. Check both scheduled and already activated promotions.
        if (await _context.VacancyPositionAssignments
            .Join(_context.VacancyPositions,
                assignment => assignment.PositionId,
                vacancyPosition => vacancyPosition.Id,
                (assignment, vacancyPosition) => new { assignment, vacancyPosition })
            .AnyAsync(x => x.assignment.EmployeeId == employeeId
                && x.vacancyPosition.VacancyId == position.VacancyId
                && (x.assignment.Status == "Scheduled" || x.assignment.Status == "Active"), cancellationToken))
        {
            throw new InvalidOperationException("This employee is already assigned to another position in the same vacancy.");
        }

        if (await _context.VacancyPositionAssignments.AnyAsync(x =>
                x.EmployeeId == employeeId && x.Status == "Scheduled", cancellationToken))
            throw new InvalidOperationException("The employee already has a scheduled vacancy assignment.");

        var sourcePositionId = await _context.VacancyPositions
            .Where(x => x.CurrentEmployeeId == employeeId && x.Status == "Filled")
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var createdAt = DateTime.UtcNow;
        var assignment = new HRVacancyPositionAssignment
        {
            PositionId = position.Id,
            EmployeeId = employee.Id,
            AssignmentType = "InternalPromotion",
            Status = "Scheduled",
            EffectiveDate = effectiveDate.Date,
            SourcePositionId = sourcePositionId,
            Remarks = Trim(remarks, 1500),
            CreatedBy = actor,
            CreatedAtUtc = createdAt
        };
        VacancyUtMetadata.Initialize(assignment, actor, systemGenerated: true, timestamp: createdAt);
        _context.VacancyPositionAssignments.Add(assignment);
        position.Status = "Reserved";
        position.UpdatedAtUtc = createdAt;
        position.UpdatedBy = actor;
        VacancyUtMetadata.Touch(position, actor, createdAt);
        AddAudit(position.VacancyId, position.Id, clientId, entityId, "InternalPromotionScheduled", actor, remarks,
            new { position.PositionNumber, EmployeeId = employeeId, EffectiveDate = effectiveDate.Date, SourcePositionId = sourcePositionId });
        await _context.SaveChangesAsync(cancellationToken);

        if (effectiveDate.Date <= DateTime.UtcNow.Date)
            await ActivateAssignmentAsync(assignment.Id, actor, cancellationToken);
        return assignment;
    }

    public async Task ActivateAssignmentAsync(int assignmentId, string actor, CancellationToken cancellationToken)
    {
        var activeStatusId = await GetLovStatusIdAsync("Active", cancellationToken);
        var inactiveStatusId = await GetLovStatusIdAsync("Inactive", cancellationToken);
        await using var transaction = _context.Database.CurrentTransaction == null
            ? await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)
            : null;
        try
        {
            var assignment = await _context.VacancyPositionAssignments
                .FirstOrDefaultAsync(x => x.Id == assignmentId, cancellationToken)
                ?? throw new InvalidOperationException("Vacancy assignment was not found.");
            if (assignment.Status == "Active") return;
            if (assignment.Status != "Scheduled")
                throw new InvalidOperationException("Only a scheduled assignment can be activated.");

            var positionAndVacancy = await _context.VacancyPositions
                .Join(_context.Vacancies, p => p.VacancyId, v => v.Id, (p, v) => new { Position = p, Vacancy = v })
                .FirstOrDefaultAsync(x => x.Position.Id == assignment.PositionId, cancellationToken)
                ?? throw new InvalidOperationException("The target vacancy position was not found.");
            var target = positionAndVacancy.Position;
            var vacancy = positionAndVacancy.Vacancy;
            if (target.Status != "Reserved")
                throw new InvalidOperationException("The target position is no longer reserved for this promotion.");

            var effectiveDate = assignment.EffectiveDate.Date;
            var employee = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(x => x.Id == assignment.EmployeeId && x.ClientId == vacancy.ClientId && x.EntityId == vacancy.EntityId
                    && !(x.UtBlock ?? false) && x.StatusId != 126 && x.StatusId != 127
                    && (!x.ResignDate.HasValue || x.ResignDate.Value.Date >= effectiveDate), cancellationToken)
                ?? throw new InvalidOperationException("The employee is no longer active in the vacancy scope.");

            var previousDay = effectiveDate.AddDays(-1);
            var openPositions = await _context.EmployeePositions
                .Where(x => x.EmployeeId == employee.Id && !(x.UtBlock ?? false)
                    && (!x.TillDate.HasValue || x.TillDate.Value.Date >= effectiveDate))
                .ToListAsync(cancellationToken);
            foreach (var row in openPositions)
            {
                row.TillDate = previousDay;
                row.UtEditTime = DateTime.UtcNow;
                row.UtRecordVersion = Math.Max(row.UtRecordVersion + 1, 1);
            }

            var openEmployments = await _context.EmployeeEmployments
                .Where(x => x.EmployeeId == employee.Id && !x.UtBlock
                    && (!x.EndDate.HasValue || x.EndDate.Value.Date >= effectiveDate))
                .ToListAsync(cancellationToken);
            foreach (var row in openEmployments)
            {
                row.EndDate = previousDay;
                row.UtEditTime = DateTime.UtcNow;
                row.UtEditUser = actor;
                row.UtRecordVersion = Math.Max(row.UtRecordVersion + 1, 1);
            }

            var promotionActionId = await _context.Lovs
                .Where(x => x.Vtype == "EMACT" && !x.UtBlock && (x.Stxt == "Promotion" || x.Code == "Promotion"))
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            var nextPositionHistoryId = (await _context.EmployeePositions.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            _context.EmployeePositions.Add(new EREmpPositionLegacy
            {
                Id = nextPositionHistoryId,
                EmployeeId = employee.Id,
                DepartmentId = vacancy.DepartmentId,
                DesignationId = vacancy.DesignationId,
                GradeId = vacancy.GradeId,
                ActionId = promotionActionId,
                Remarks = Trim(assignment.Remarks, 1500),
                FromDate = effectiveDate,
                TillDate = FarFuture,
                Responsibility = Trim(vacancy.Responsibilities, 1500),
                UtBlock = false,
                UtRecordVersion = 1
            });

            // Vacancy.EmploymentTypeId is the Employee Profile EMPTP LOV ID.
            // EREmployment history uses the separate EREmploymenttype master,
            // so carry its existing value forward instead of mixing ID domains.
            var employmentTypeId = openEmployments.OrderByDescending(x => x.StartDate)
                .Select(x => x.EmploymentTypeId)
                .FirstOrDefault();
            if (employmentTypeId.HasValue)
            {
                var nextEmploymentId = (await _context.EmployeeEmployments.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                _context.EmployeeEmployments.Add(new EmployeeEmploymentLegacy
                {
                    Id = nextEmploymentId,
                    ClientId = vacancy.ClientId,
                    EntityId = vacancy.EntityId,
                    EmployeeId = employee.Id,
                    EmploymentTypeId = employmentTypeId,
                    StartDate = effectiveDate,
                    EndDate = FarFuture,
                    UtSysGen = true,
                    UtIsPartial = false,
                    UtAddTime = DateTime.UtcNow,
                    UtAddUser = actor,
                    UtRecordVersion = 1,
                    UtBlock = false
                });
            }

            employee.DepartmentId = vacancy.DepartmentId;
            employee.DesignationId = vacancy.DesignationId;
            employee.GradeId = vacancy.GradeId;
            employee.EmployeeTypeId = vacancy.EmploymentTypeId ?? employee.EmployeeTypeId;
            employee.UtEditTime = DateTime.UtcNow;
            employee.UtRecordVersion = Math.Max(employee.UtRecordVersion + 1, 1);

            if (assignment.SourcePositionId.HasValue)
            {
                var source = await _context.VacancyPositions.FirstOrDefaultAsync(x => x.Id == assignment.SourcePositionId.Value, cancellationToken);
                if (source != null)
                {
                    source.Status = "Vacant";
                    source.CurrentEmployeeId = null;
                    source.NeedsReview = true;
                    source.ReviewReason = "Vacated by internal promotion; HR must decide whether to refill or cancel this position.";
                    source.UpdatedAtUtc = DateTime.UtcNow;
                    source.UpdatedBy = actor;
                    VacancyUtMetadata.Touch(source, actor, source.UpdatedAtUtc);
                    var sourceCanonicalAssignment = await _context.PositionAssignments.FirstOrDefaultAsync(x =>
                        x.PositionId == source.PositionId && x.EmployeeId == employee.Id
                        && x.LovStatusId == activeStatusId && x.EffectiveTo == null, cancellationToken);
                    if (sourceCanonicalAssignment != null)
                    {
                        sourceCanonicalAssignment.LovStatusId = inactiveStatusId;
                        sourceCanonicalAssignment.EffectiveTo = effectiveDate;
                        Touch(sourceCanonicalAssignment, actor);
                        AddPositionAudit(source.PositionId, vacancy.ClientId, vacancy.EntityId, "PositionVacatedByPromotion", actor,
                            new { EmployeeId = employee.Id }, null);
                    }
                }
            }

            if (await _context.PositionAssignments.AnyAsync(x => x.PositionId == target.PositionId
                && x.LovStatusId == activeStatusId && x.EffectiveTo == null, cancellationToken))
                throw new InvalidOperationException("The canonical target position is no longer vacant.");
            var targetExisting = await _context.PositionAssignments.FirstOrDefaultAsync(x =>
                x.EmployeeId == employee.Id && x.LovStatusId == activeStatusId && x.EffectiveTo == null, cancellationToken);
            if (targetExisting != null)
            {
                targetExisting.LovStatusId = inactiveStatusId;
                targetExisting.EffectiveTo = effectiveDate;
                Touch(targetExisting, actor);
            }
            _context.PositionAssignments.Add(NewCanonicalAssignment(target.PositionId, employee.Id, "InternalPromotion",
                effectiveDate, activeStatusId, actor, assignment.Remarks));
            AddPositionAudit(target.PositionId, vacancy.ClientId, vacancy.EntityId, "PositionOccupiedByPromotion", actor, null,
                new { EmployeeId = employee.Id, EffectiveDate = effectiveDate });

            target.Status = "Filled";
            target.CurrentEmployeeId = employee.Id;
            target.NeedsReview = false;
            target.ReviewReason = null;
            target.UpdatedAtUtc = DateTime.UtcNow;
            target.UpdatedBy = actor;
            VacancyUtMetadata.Touch(target, actor, target.UpdatedAtUtc);
            assignment.Status = "Active";
            assignment.ActivatedAtUtc = DateTime.UtcNow;
            VacancyUtMetadata.Touch(assignment, actor, assignment.ActivatedAtUtc);
            AddAudit(vacancy.Id, target.Id, vacancy.ClientId, vacancy.EntityId, "InternalPromotionActivated", actor, assignment.Remarks,
                new { target.PositionNumber, EmployeeId = employee.Id, EffectiveDate = effectiveDate, assignment.SourcePositionId });
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction != null) await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            if (transaction != null) await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<int> ReleaseEmployeePositionsAsync(
        int employeeId,
        int clientId,
        int entityId,
        DateTime effectiveDate,
        string actor,
        string reason,
        CancellationToken cancellationToken)
    {
        var activeStatusId = await GetLovStatusIdAsync("Active", cancellationToken);
        var inactiveStatusId = await GetLovStatusIdAsync("Inactive", cancellationToken);
        var releaseDate = effectiveDate.Date;
        var assignments = await _context.PositionAssignments
            .Join(_context.Positions,
                assignment => assignment.PositionId,
                position => position.Id,
                (assignment, position) => new { Assignment = assignment, Position = position })
            .Where(x => x.Assignment.EmployeeId == employeeId
                && x.Assignment.LovStatusId == activeStatusId
                && x.Assignment.EffectiveTo == null
                && x.Position.ClientId == clientId
                && x.Position.EntityId == entityId
                && x.Position.LovStatusId == activeStatusId)
            .ToListAsync(cancellationToken);

        if (assignments.Count == 0) return 0;
        if (assignments.Any(x => releaseDate < x.Assignment.EffectiveFrom.Date))
            throw new InvalidOperationException("Leaving date cannot be before the employee's active position assignment date.");

        var canonicalPositionIds = assignments.Select(x => x.Position.Id).Distinct().ToList();
        var vacancyPositions = await _context.VacancyPositions
            .Where(x => canonicalPositionIds.Contains(x.PositionId)
                && x.CurrentEmployeeId == employeeId
                && x.Status == "Filled")
            .ToListAsync(cancellationToken);
        var vacancyPositionIds = vacancyPositions.Select(x => x.Id).ToList();
        var vacancyAssignments = await _context.VacancyPositionAssignments
            .Where(x => vacancyPositionIds.Contains(x.PositionId)
                && x.EmployeeId == employeeId
                && x.Status == "Active")
            .ToListAsync(cancellationToken);
        var legacyPositions = await _context.EmployeePositions
            .Where(x => x.EmployeeId == employeeId && !(x.UtBlock ?? false)
                && (!x.TillDate.HasValue || x.TillDate.Value.Date > releaseDate))
            .ToListAsync(cancellationToken);

        foreach (var item in assignments)
        {
            item.Assignment.LovStatusId = inactiveStatusId;
            item.Assignment.EffectiveTo = releaseDate;
            item.Assignment.Remarks = Trim(reason, 1500) ?? item.Assignment.Remarks;
            Touch(item.Assignment, actor);
            AddPositionAudit(item.Position.Id, clientId, entityId, "PositionVacatedByEmployeeExit", actor,
                new { EmployeeId = employeeId }, new { EffectiveTo = releaseDate, Reason = reason });
        }

        foreach (var position in vacancyPositions)
        {
            position.Status = "Cancelled";
            position.CurrentEmployeeId = null;
            position.NeedsReview = false;
            position.ReviewReason = Trim($"Position vacated on {releaseDate:yyyy-MM-dd}. Create a Replacement vacancy to refill it.", 500);
            position.UpdatedAtUtc = DateTime.UtcNow;
            position.UpdatedBy = actor;
            VacancyUtMetadata.Touch(position, actor, position.UpdatedAtUtc);
            AddAudit(position.VacancyId, position.Id, clientId, entityId, "PositionVacatedByEmployeeExit", actor, reason,
                new { position.PositionNumber, EmployeeId = employeeId, EffectiveTo = releaseDate });
        }

        foreach (var assignment in vacancyAssignments)
        {
            assignment.Status = "Ended";
            assignment.Remarks = Trim(reason, 1500) ?? assignment.Remarks;
            VacancyUtMetadata.Touch(assignment, actor);
        }

        foreach (var position in legacyPositions)
        {
            position.TillDate = releaseDate;
            position.UtEditTime = DateTime.UtcNow;
            position.UtRecordVersion = Math.Max(position.UtRecordVersion + 1, 1);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return assignments.Count;
    }

    public async Task<int> ReleaseDueEmployeeExitsAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var activeStatusId = await GetLovStatusIdAsync("Active", cancellationToken);
        var dueEmployees = await _context.PositionAssignments.AsNoTracking()
            .Join(_context.Positions.AsNoTracking(),
                assignment => assignment.PositionId,
                position => position.Id,
                (assignment, position) => new { Assignment = assignment, Position = position })
            .Join(_context.EmployeeProfiles.AsNoTracking(),
                item => item.Assignment.EmployeeId,
                employee => employee.Id,
                (item, employee) => new { item.Assignment, item.Position, Employee = employee })
            .Where(x => x.Assignment.LovStatusId == activeStatusId
                && x.Assignment.EffectiveTo == null
                && x.Position.EntityId.HasValue
                && (x.Employee.StatusId == 126 || x.Employee.StatusId == 127)
                && x.Employee.ResignDate.HasValue
                && x.Employee.ResignDate.Value.Date <= today
                && !(x.Employee.UtBlock ?? false))
            .Select(x => new
            {
                EmployeeId = x.Employee.Id,
                x.Position.ClientId,
                EntityId = x.Position.EntityId!.Value,
                EffectiveDate = x.Employee.ResignDate!.Value
            })
            .Where(x => x.EntityId > 0)
            .Distinct()
            .ToListAsync(cancellationToken);

        var released = 0;
        foreach (var employee in dueEmployees)
        {
            try
            {
                released += await ReleaseEmployeePositionsAsync(
                    employee.EmployeeId,
                    employee.ClientId,
                    employee.EntityId,
                    employee.EffectiveDate,
                    "Employee exit scheduler",
                    "Employee resigned",
                    cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to release positions for resigned employee {EmployeeId}.", employee.EmployeeId);
                _context.ChangeTracker.Clear();
            }
        }
        return released;
    }

    public async Task<int> ActivateDuePromotionsAsync(CancellationToken cancellationToken)
    {
        var dueIds = await _context.VacancyPositionAssignments.AsNoTracking()
            .Where(x => x.Status == "Scheduled" && x.AssignmentType == "InternalPromotion" && x.EffectiveDate.Date <= DateTime.UtcNow.Date)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var activated = 0;
        foreach (var id in dueIds)
        {
            try
            {
                await ActivateAssignmentAsync(id, "Vacancy promotion scheduler", cancellationToken);
                activated++;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to activate scheduled vacancy assignment {AssignmentId}", id);
                _context.ChangeTracker.Clear();
                var failed = await _context.VacancyPositionAssignments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (failed != null)
                {
                    failed.Status = "Failed";
                    failed.FailureReason = Trim(exception.Message, 1500);
                    VacancyUtMetadata.Touch(failed, "Vacancy promotion scheduler");
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
        return activated;
    }

    private void AddAudit(int vacancyId, int? positionId, int clientId, int entityId, string action, string actor, string? remarks, object? after)
    {
        var occurredAt = DateTime.UtcNow;
        var audit = new HRVacancyAudit
        {
            VacancyId = vacancyId,
            PositionId = positionId,
            ClientId = clientId,
            EntityId = entityId,
            Action = action,
            Actor = actor,
            Remarks = Trim(remarks, 1500),
            AfterJson = after == null ? null : JsonSerializer.Serialize(after),
            OccurredAtUtc = occurredAt
        };
        VacancyUtMetadata.Initialize(audit, actor, systemGenerated: true, timestamp: occurredAt);
        _context.VacancyAudits.Add(audit);
    }

    private static HRPositionAssignment NewCanonicalAssignment(
        int positionId, int employeeId, string type, DateTime effectiveFrom,
        int activeStatusId, string actor, string? remarks = null)
    {
        var shortActor = Short(actor, 25);
        return new HRPositionAssignment
        {
            PositionId = positionId, EmployeeId = employeeId, AssignmentType = type, LovStatusId = activeStatusId,
            EffectiveFrom = effectiveFrom, Remarks = Trim(remarks, 1500),
            UtDocType = 0, UtSubDocType = 0, UtSysGen = true,
            UtAddTime = DateTime.UtcNow, UtAddUser = shortActor, UtAddSystem = "HR.Service", UtAddIp = string.Empty,
            UtAddAppVersion = string.Empty, UtAddDbVersion = string.Empty, UtAddId = 0, UtAddSessionId = 0,
            UtEditTime = DateTime.UtcNow, UtEditUser = shortActor, UtEditSystem = "HR.Service", UtEditIp = string.Empty,
            UtEditAppVersion = string.Empty, UtEditDbVersion = string.Empty, UtEditId = 0, UtEditSessionId = 0
        };
    }

    private void AddPositionAudit(int positionId, int clientId, int? entityId, string action, string actor, object? before, object? after)
    {
        var now = DateTime.UtcNow;
        var shortActor = Short(actor, 25);
        _context.PositionAudits.Add(new HRPositionAudit
        {
            PositionId = positionId, ClientId = clientId, EntityId = entityId, Action = action,
            Actor = Short(actor, 100), BeforeJson = before == null ? null : JsonSerializer.Serialize(before),
            AfterJson = after == null ? null : JsonSerializer.Serialize(after), OccurredAtUtc = now,
            UtDocType = 0, UtSubDocType = 0, UtSysGen = true,
            UtAddTime = now, UtAddUser = shortActor, UtAddSystem = "HR.Service", UtAddIp = string.Empty,
            UtAddAppVersion = string.Empty, UtAddDbVersion = string.Empty, UtAddId = 0, UtAddSessionId = 0,
            UtEditTime = now, UtEditUser = shortActor, UtEditSystem = "HR.Service", UtEditIp = string.Empty,
            UtEditAppVersion = string.Empty, UtEditDbVersion = string.Empty, UtEditId = 0, UtEditSessionId = 0
        });
    }

    private static void Touch(HRPositionAssignment assignment, string actor)
    {
        assignment.UtEditTime = DateTime.UtcNow;
        assignment.UtEditUser = Short(actor, 25);
        assignment.UtEditSystem = "HR.Service";
    }

    private static string Short(string value, int length) => value[..Math.Min(value.Length, length)];

    private async Task<int> GetLovStatusIdAsync(string status, CancellationToken cancellationToken)
    {
        var code = status.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "ACT" : "INA";
        var id = await _context.Lovs.AsNoTracking()
            .Where(x => x.Vtype == "AACT" && !x.UtBlock && (x.Code == code || x.Stxt == status))
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return id ?? throw new InvalidOperationException($"Required {status} status LOV (AACT: {code}) is not configured.");
    }

    private static string? Trim(string? value, int length)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized[..Math.Min(length, normalized.Length)];
    }
}
