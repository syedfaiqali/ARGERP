using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class ShiftCommandHandlers :
        IRequestHandler<CreateShiftV3Command, ShiftHeaderV3Dto>,
        IRequestHandler<UpdateShiftV3Command, bool>,
        IRequestHandler<DeleteShiftV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public ShiftCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<ShiftHeaderV3Dto> Handle(CreateShiftV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequest(request);
            await ApplyEmployeeScheduleContinuityAsync(request.ShiftDetails, request.SharedDetails, null, command.ClientId, cancellationToken);

            var nameExists = await _context.Shifts
                .AnyAsync(x => x.Stxt == request.Stxt.Trim()
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Shift Name already exists.");
            }

            var nextId = (await _context.Shifts.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var code = string.IsNullOrWhiteSpace(request.Code)
                ? nextId.ToString("0000000000")
                : request.Code.Trim();

            var validAbsPolicyId = await ResolveValidShiftHeaderIdAsync(
                request.AbsPolicyId,
                command.ClientId,
                command.EntityId,
                cancellationToken);

            var entity = new ShiftLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.Stxt.Trim(),
                ShiftTypeId = request.ShiftTypeId,
                LovSharedId = request.LovSharedId,
                WoMonday = request.WoMonday,
                WoTuesday = request.WoTuesday,
                WoWednesday = request.WoWednesday,
                WoThuresday = request.WoThuresday,
                WoFriday = request.WoFriday,
                WoSaturday = request.WoSaturday,
                WoSunday = request.WoSunday,
                LateArrivPolicyId = NormalizeId(request.LateArrivPolicyId),
                EarlyDepPolicyId = NormalizeId(request.EarlyDepPolicyId),
                OtPolicyId = NormalizeId(request.OtPolicyId),
                AbsPolicyId = validAbsPolicyId,
                LateArrivPolicyMnt = request.LateArrivPolicyMnt,
                EarlyDepPolicyMnt = request.EarlyDepPolicyMnt,
                OtPolicyMnt = request.OtPolicyMnt,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtBlock = !request.IsActive
            };

            _context.Shifts.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var nextDetailId = await GetNextShiftDetailId(cancellationToken);
            var nextSubId = await GetNextShiftSubDetailId(cancellationToken);

            foreach (var detail in request.ShiftDetails)
            {
                var detailEntity = new ETShiftdtlLegacy
                {
                    Id = nextDetailId++,
                    ShiftId = entity.Id,
                    LineIndex = detail.LineIndex > 0 ? detail.LineIndex : nextDetailId - 1,
                    DayId = detail.DayId,
                    InTime = detail.InTime,
                    OutTime = detail.OutTime,
                    StartDate = detail.StartDate,
                    EndDate = detail.EndDate,
                    GraceTime = detail.GraceTime,
                    GraceTimeEnd = detail.GraceTimeEnd,
                    LatePolicyApplicable = detail.LatePolicyApplicable,
                    EarlyPolicyApplicable = detail.EarlyPolicyApplicable,
                    Duration = detail.Duration,
                    UtBlock = false
                };

                _context.ShiftDetails.Add(detailEntity);

                foreach (var br in detail.Breaks ?? new List<ShiftBreakV3Dto>())
                {
                    _context.ShiftSubDetails.Add(new ETShiftsubdtlLegacy
                    {
                        Id = nextSubId++,
                        ShiftDtlId = detailEntity.Id,
                        LineIndex = br.LineIndex > 0 ? br.LineIndex : nextSubId - 1,
                        Stxt = br.Stxt,
                        StartTime = br.StartTime,
                        EndTime = br.EndTime,
                        Duration = br.Duration,
                        UtBlock = false
                    });
                }
            }

            var nextSharedId = await GetNextShiftSharedDetailId(cancellationToken);
            var validEmployeeIds = await ResolveValidEmployeeIdsAsync(
                request.SharedDetails,
                command.ClientId,
                command.EntityId,
                cancellationToken);
            var validDepartmentIds = await ResolveValidDepartmentIdsAsync(
                request.SharedDetails,
                cancellationToken);
            foreach (var shared in request.SharedDetails)
            {
                var entityId = NormalizeId(shared.EntityId);
                var departmentId = NormalizeId(shared.DepartmentId);
                var employeeId = NormalizeId(shared.EmployeeId);
                if (employeeId.HasValue && !validEmployeeIds.Contains(employeeId.Value))
                {
                    employeeId = null;
                }
                if (departmentId.HasValue && !validDepartmentIds.Contains(departmentId.Value))
                {
                    departmentId = null;
                }
                if (entityId == null && departmentId == null && employeeId == null)
                {
                    continue;
                }
                _context.ShiftSharedDetails.Add(new ETShiftshareddtlLegacy
                {
                    Id = nextSharedId++,
                    LineIndex = shared.LineIndex > 0 ? shared.LineIndex : nextSharedId - 1,
                    ShiftId = entity.Id,
                    LovSharedId = shared.LovSharedId ?? request.LovSharedId,
                    PClientId = command.ClientId,
                    PEntityId = entityId,
                    PDepartId = departmentId,
                    PEmpId = employeeId,
                    UtBlock = false
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ShiftHeaderV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.ShiftTypeId,
                null,
                entity.LovSharedId,
                null,
                entity.WoMonday ?? false,
                entity.WoTuesday ?? false,
                entity.WoWednesday ?? false,
                entity.WoThuresday ?? false,
                entity.WoFriday ?? false,
                entity.WoSaturday ?? false,
                entity.WoSunday ?? false,
                entity.LateArrivPolicyId,
                entity.EarlyDepPolicyId,
                entity.OtPolicyId,
                entity.AbsPolicyId,
                entity.LateArrivPolicyMnt,
                entity.EarlyDepPolicyMnt,
                entity.OtPolicyMnt,
                !(entity.UtBlock ?? false),
                request.ShiftDetails,
                request.SharedDetails
            );
        }

        public async Task<bool> Handle(UpdateShiftV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequest(request);

            // Shared shifts can be opened outside their creator entity. Honor the
            // same sharing targets as the setup queries when locating one to edit.
            var entity = await _context.Shifts
                .FirstOrDefaultAsync(x => x.Id == request.Id
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == null || x.EntityId == command.EntityId.Value ||
                        _context.ShiftSharedDetails.Any(shared =>
                            shared.ShiftId == x.Id &&
                            shared.PClientId == command.ClientId &&
                            (shared.PDepartId.HasValue ||
                             shared.PEntityId == command.EntityId.Value ||
                             (shared.PEmpId.HasValue &&
                              _context.EmployeeProfiles.Any(employee =>
                                  employee.Id == shared.PEmpId.Value &&
                                  employee.ClientId == command.ClientId &&
                                  employee.EntityId == command.EntityId.Value))))), cancellationToken);

            if (entity == null) return false;

            var nameExists = await _context.Shifts
                .AnyAsync(x => x.Stxt == request.Stxt.Trim()
                    && x.Id != request.Id
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == null || x.EntityId == command.EntityId.Value), cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Shift Name already exists.");
            }

            await ApplyEmployeeScheduleContinuityAsync(request.ShiftDetails, request.SharedDetails, entity.Id, command.ClientId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                entity.Code = request.Code.Trim();
            }

            entity.Stxt = request.Stxt.Trim();
            entity.ShiftTypeId = request.ShiftTypeId;
            entity.LovSharedId = request.LovSharedId;
            entity.WoMonday = request.WoMonday;
            entity.WoTuesday = request.WoTuesday;
            entity.WoWednesday = request.WoWednesday;
            entity.WoThuresday = request.WoThuresday;
            entity.WoFriday = request.WoFriday;
            entity.WoSaturday = request.WoSaturday;
            entity.WoSunday = request.WoSunday;
            entity.LateArrivPolicyId = NormalizeId(request.LateArrivPolicyId);
            entity.EarlyDepPolicyId = NormalizeId(request.EarlyDepPolicyId);
            entity.OtPolicyId = NormalizeId(request.OtPolicyId);
            entity.AbsPolicyId = await ResolveValidShiftHeaderIdAsync(
                request.AbsPolicyId,
                command.ClientId,
                command.EntityId,
                cancellationToken);
            entity.LateArrivPolicyMnt = request.LateArrivPolicyMnt;
            entity.EarlyDepPolicyMnt = request.EarlyDepPolicyMnt;
            entity.OtPolicyMnt = request.OtPolicyMnt;
            entity.UtBlock = !request.IsActive;

            var existingDetails = await _context.ShiftDetails
                .Where(x => x.ShiftId == entity.Id)
                .ToListAsync(cancellationToken);

            var detailIds = existingDetails.Select(x => x.Id).ToList();
            var existingSub = await _context.ShiftSubDetails
                .Where(x => detailIds.Contains(x.ShiftDtlId ?? 0))
                .ToListAsync(cancellationToken);

            _context.ShiftSubDetails.RemoveRange(existingSub);
            _context.ShiftDetails.RemoveRange(existingDetails);

            var existingShared = await _context.ShiftSharedDetails
                .Where(x => x.ShiftId == entity.Id)
                .ToListAsync(cancellationToken);
            _context.ShiftSharedDetails.RemoveRange(existingShared);

            var nextDetailId = await GetNextShiftDetailId(cancellationToken);
            var nextSubId = await GetNextShiftSubDetailId(cancellationToken);

            foreach (var detail in request.ShiftDetails)
            {
                var detailEntity = new ETShiftdtlLegacy
                {
                    Id = AllocatePositiveId(detail.Id, ref nextDetailId),
                    ShiftId = entity.Id,
                    LineIndex = detail.LineIndex > 0 ? detail.LineIndex : nextDetailId - 1,
                    DayId = detail.DayId,
                    InTime = detail.InTime,
                    OutTime = detail.OutTime,
                    StartDate = detail.StartDate,
                    EndDate = detail.EndDate,
                    GraceTime = detail.GraceTime,
                    GraceTimeEnd = detail.GraceTimeEnd,
                    LatePolicyApplicable = detail.LatePolicyApplicable,
                    EarlyPolicyApplicable = detail.EarlyPolicyApplicable,
                    Duration = detail.Duration,
                    UtBlock = false
                };

                _context.ShiftDetails.Add(detailEntity);

                foreach (var br in detail.Breaks ?? new List<ShiftBreakV3Dto>())
                {
                    _context.ShiftSubDetails.Add(new ETShiftsubdtlLegacy
                    {
                        Id = AllocatePositiveId(br.Id, ref nextSubId),
                        ShiftDtlId = detailEntity.Id,
                        LineIndex = br.LineIndex > 0 ? br.LineIndex : nextSubId - 1,
                        Stxt = br.Stxt,
                        StartTime = br.StartTime,
                        EndTime = br.EndTime,
                        Duration = br.Duration,
                        UtBlock = false
                    });
                }
            }

            var nextSharedId = await GetNextShiftSharedDetailId(cancellationToken);
            var validEmployeeIds = await ResolveValidEmployeeIdsAsync(
                request.SharedDetails,
                command.ClientId,
                command.EntityId,
                cancellationToken);
            var validDepartmentIds = await ResolveValidDepartmentIdsAsync(
                request.SharedDetails,
                cancellationToken);
            foreach (var shared in request.SharedDetails)
            {
                var entityId = NormalizeId(shared.EntityId);
                var departmentId = NormalizeId(shared.DepartmentId);
                var employeeId = NormalizeId(shared.EmployeeId);
                if (employeeId.HasValue && !validEmployeeIds.Contains(employeeId.Value))
                {
                    employeeId = null;
                }
                if (departmentId.HasValue && !validDepartmentIds.Contains(departmentId.Value))
                {
                    departmentId = null;
                }
                if (entityId == null && departmentId == null && employeeId == null)
                {
                    continue;
                }
                _context.ShiftSharedDetails.Add(new ETShiftshareddtlLegacy
                {
                    Id = AllocatePositiveId(shared.Id, ref nextSharedId),
                    LineIndex = shared.LineIndex > 0 ? shared.LineIndex : nextSharedId - 1,
                    ShiftId = entity.Id,
                    LovSharedId = shared.LovSharedId ?? request.LovSharedId,
                    PClientId = command.ClientId,
                    PEntityId = entityId,
                    PDepartId = departmentId,
                    PEmpId = employeeId,
                    UtBlock = false
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static int? NormalizeId(int? value)
        {
            return value.HasValue && value.Value > 0 ? value : null;
        }

        private static int AllocatePositiveId(int? requestedId, ref int nextId)
        {
            if (requestedId.HasValue && requestedId.Value > 0)
            {
                return requestedId.Value;
            }

            return nextId++;
        }

        private async Task<int> GetNextShiftDetailId(CancellationToken cancellationToken)
        {
            return ((await _context.ShiftDetails.IgnoreQueryFilters()
                .Where(x => x.Id > 0)
                .MaxAsync(x => (int?)x.Id, cancellationToken)) ?? 0) + 1;
        }

        private async Task<int> GetNextShiftSubDetailId(CancellationToken cancellationToken)
        {
            return ((await _context.ShiftSubDetails.IgnoreQueryFilters()
                .Where(x => x.Id > 0)
                .MaxAsync(x => (int?)x.Id, cancellationToken)) ?? 0) + 1;
        }

        private async Task<int> GetNextShiftSharedDetailId(CancellationToken cancellationToken)
        {
            return ((await _context.ShiftSharedDetails.IgnoreQueryFilters()
                .Where(x => x.Id > 0)
                .MaxAsync(x => (int?)x.Id, cancellationToken)) ?? 0) + 1;
        }

        private async Task<HashSet<int>> ResolveValidEmployeeIdsAsync(
            List<ShiftSharedWithDetailV3Dto> sharedDetails,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (sharedDetails == null || sharedDetails.Count == 0)
                return new HashSet<int>();

            var candidateIds = sharedDetails
                .Select(s => NormalizeId(s.EmployeeId))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            if (candidateIds.Count == 0)
                return new HashSet<int>();

            return await _context.EmployeeProfiles.AsNoTracking()
                .Where(e => candidateIds.Contains(e.Id)
                    && e.ClientId == clientId
                    && (!entityId.HasValue || e.EntityId == entityId.Value))
                .Select(e => e.Id)
                .ToHashSetAsync(cancellationToken);
        }

        private async Task<HashSet<int>> ResolveValidDepartmentIdsAsync(
            List<ShiftSharedWithDetailV3Dto> sharedDetails,
            CancellationToken cancellationToken)
        {
            if (sharedDetails == null || sharedDetails.Count == 0)
                return new HashSet<int>();

            var candidateIds = sharedDetails
                .Select(s => NormalizeId(s.DepartmentId))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            if (candidateIds.Count == 0)
                return new HashSet<int>();

            return await _context.Departments.AsNoTracking()
                .Where(d => candidateIds.Contains(d.Id))
                .Select(d => d.Id)
                .ToHashSetAsync(cancellationToken);
        }

        private async Task<int?> ResolveValidShiftHeaderIdAsync(
            int? shiftId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (!shiftId.HasValue || shiftId.Value <= 0)
            {
                return null;
            }

            var exists = await _context.Shifts.AsNoTracking()
                .AnyAsync(s => s.Id == shiftId.Value
                    && s.ClientId == clientId
                    && (!entityId.HasValue || s.EntityId == entityId.Value),
                    cancellationToken);

            return exists ? shiftId : null;
        }

        private async Task ApplyEmployeeScheduleContinuityAsync(
            List<ShiftDetailV3Dto> newDetails,
            List<ShiftSharedWithDetailV3Dto> sharedDetails,
            int? currentShiftId,
            int clientId,
            CancellationToken cancellationToken)
        {
            var employeeIds = (sharedDetails ?? new List<ShiftSharedWithDetailV3Dto>())
                .Select(detail => NormalizeId(detail.EmployeeId))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();
            if (employeeIds.Count == 0)
                return;

            var existingSchedules = await (
                from detail in _context.ShiftDetails
                join shared in _context.ShiftSharedDetails on detail.ShiftId equals shared.ShiftId
                join shift in _context.Shifts on detail.ShiftId equals shift.Id
                where employeeIds.Contains(shared.PEmpId ?? 0)
                      && shift.ClientId == clientId
                      && (!currentShiftId.HasValue || detail.ShiftId != currentShiftId.Value)
                select detail)
                .ToListAsync(cancellationToken);

            foreach (var newDetail in newDetails.Where(detail => detail.StartDate.HasValue))
            {
                var effectiveDate = newDetail.StartDate!.Value.Date;
                var newEndDate = newDetail.EndDate?.Date;
                var sameEffectiveDate = existingSchedules.Any(existing =>
                    existing.DayId == newDetail.DayId &&
                    existing.StartDate.HasValue &&
                    existing.StartDate.Value.Date == effectiveDate);
                if (sameEffectiveDate)
                    throw new InvalidOperationException("A schedule with the same Effective Date already exists for the selected employee.");

                var overlapsFutureSchedule = newEndDate.HasValue && existingSchedules.Any(existing =>
                    existing.DayId == newDetail.DayId &&
                    existing.StartDate.HasValue &&
                    existing.StartDate.Value.Date > effectiveDate &&
                    existing.StartDate.Value.Date <= newEndDate.Value);
                if (overlapsFutureSchedule)
                    throw new InvalidOperationException("Schedule date range overlaps another schedule for the selected employee.");

                foreach (var previous in existingSchedules.Where(existing =>
                    existing.DayId == newDetail.DayId &&
                    existing.StartDate.HasValue &&
                    existing.StartDate.Value.Date < effectiveDate &&
                    (!existing.EndDate.HasValue || existing.EndDate.Value.Date >= effectiveDate)))
                {
                    previous.EndDate = effectiveDate.AddDays(-1);
                }
            }
        }

        public async Task<bool> Handle(DeleteShiftV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.Shifts
                .FirstOrDefaultAsync(x => x.Id == command.Id
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == null || x.EntityId == command.EntityId.Value ||
                        _context.ShiftSharedDetails.Any(shared =>
                            shared.ShiftId == x.Id &&
                            shared.PClientId == command.ClientId &&
                            shared.PDepartId.HasValue)), cancellationToken);

            if (entity == null) return false;

            var detailIds = await _context.ShiftDetails
                .Where(x => x.ShiftId == entity.Id)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var isInUse = await _context.EmployeeRosters
                .AnyAsync(x => x.ShiftId == entity.Id, cancellationToken)
                || await _context.EmployeeProfiles
                    .AnyAsync(x => x.ShiftId == entity.Id, cancellationToken)
                || await _context.Schedules
                    .AnyAsync(x => x.ShiftId == entity.Id, cancellationToken);

            if (isInUse)
            {
                throw new InvalidOperationException(
                    "This shift is assigned to an employee, roster, or schedule and cannot be deleted.");
            }

            var subDetails = await _context.ShiftSubDetails
                .Where(x => detailIds.Contains(x.ShiftDtlId ?? 0))
                .ToListAsync(cancellationToken);
            var details = await _context.ShiftDetails
                .Where(x => x.ShiftId == entity.Id)
                .ToListAsync(cancellationToken);
            var sharedDetails = await _context.ShiftSharedDetails
                .Where(x => x.ShiftId == entity.Id)
                .ToListAsync(cancellationToken);

            _context.ShiftSubDetails.RemoveRange(subDetails);
            _context.ShiftDetails.RemoveRange(details);
            _context.ShiftSharedDetails.RemoveRange(sharedDetails);
            _context.Shifts.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static void ValidateRequest(CreateShiftV3Request request)
        {
            if (request.ShiftTypeId == null)
                throw new InvalidOperationException("Shift Type is required.");
            if (string.IsNullOrWhiteSpace(request.Stxt))
                throw new InvalidOperationException("Shift Name is required.");
            if (request.Stxt.Trim().Length > 50)
                throw new InvalidOperationException("Shift Name cannot exceed 50 characters.");
            if (!string.IsNullOrWhiteSpace(request.Code) && request.Code.Trim().Length > 50)
                throw new InvalidOperationException("Code cannot exceed 50 characters.");
            if (request.OtPolicyMnt.HasValue && request.OtPolicyMnt <= 0)
                throw new InvalidOperationException("Overtime Policy Minutes must be greater than zero.");
            ValidateDetails(request.ShiftDetails, request.SharedDetails);
        }

        private static void ValidateRequest(UpdateShiftV3Request request)
        {
            if (request.Id <= 0)
                throw new InvalidOperationException("Invalid Shift Id.");
            if (request.ShiftTypeId == null)
                throw new InvalidOperationException("Shift Type is required.");
            if (string.IsNullOrWhiteSpace(request.Stxt))
                throw new InvalidOperationException("Shift Name is required.");
            if (request.Stxt.Trim().Length > 50)
                throw new InvalidOperationException("Shift Name cannot exceed 50 characters.");
            if (!string.IsNullOrWhiteSpace(request.Code) && request.Code.Trim().Length > 50)
                throw new InvalidOperationException("Code cannot exceed 50 characters.");
            if (request.OtPolicyMnt.HasValue && request.OtPolicyMnt <= 0)
                throw new InvalidOperationException("Overtime Policy Minutes must be greater than zero.");
            ValidateDetails(request.ShiftDetails, request.SharedDetails);
        }

        private static void ValidateDetails(List<ShiftDetailV3Dto> details, List<ShiftSharedWithDetailV3Dto> sharedDetails)
        {
            if (details == null || details.Count == 0)
                throw new InvalidOperationException("Schedule is required.");

            foreach (var detail in details)
            {
                if (detail.DayId <= 0)
                    throw new InvalidOperationException("Day is required in Schedule.");
                if (detail.InTime == null)
                    throw new InvalidOperationException("In Time is required in Schedule.");
                if (detail.OutTime == null)
                    throw new InvalidOperationException("Out Time is required in Schedule.");
                if (detail.StartDate == null)
                    throw new InvalidOperationException("Start Date is required in Schedule.");
                if (detail.EndDate == null)
                    throw new InvalidOperationException("End Date is required in Schedule.");
                if (detail.EndDate.Value.Date < detail.StartDate.Value.Date)
                    throw new InvalidOperationException("End Date cannot be less than Start Date.");

                var breaks = detail.Breaks ?? new List<ShiftBreakV3Dto>();
                foreach (var br in breaks)
                {
                    if (!string.IsNullOrWhiteSpace(br.Stxt) && br.Stxt.Length > 49)
                        throw new InvalidOperationException("Break Name cannot exceed 49 characters.");
                    if (br.StartTime == null || br.EndTime == null)
                        throw new InvalidOperationException("Break Start Time and End Time are required.");
                    if (br.EndTime < br.StartTime)
                        throw new InvalidOperationException("Break End Time cannot be less than Start Time.");
                    if (detail.InTime.HasValue && br.StartTime < detail.InTime)
                        throw new InvalidOperationException("Break Start Time cannot be less than Schedule In Time.");
                    if (detail.OutTime.HasValue && br.EndTime > detail.OutTime)
                        throw new InvalidOperationException("Break End Time cannot be greater than Schedule Out Time.");
                }

                for (var i = 0; i < breaks.Count; i++)
                {
                    var currentBreak = breaks[i];
                    if (!currentBreak.StartTime.HasValue || !currentBreak.EndTime.HasValue) continue;

                    if (breaks.Take(i).Any(previousBreak =>
                        previousBreak.StartTime.HasValue && previousBreak.EndTime.HasValue &&
                        RangesOverlap(
                            previousBreak.StartTime.Value,
                            previousBreak.EndTime.Value,
                            currentBreak.StartTime.Value,
                            currentBreak.EndTime.Value)))
                    {
                        throw new InvalidOperationException("Break time overlaps another break in the same schedule.");
                    }
                }
            }

            if (sharedDetails == null || sharedDetails.Count == 0)
                throw new InvalidOperationException("Shared With Detail is required.");
            if (!sharedDetails.Any(detail =>
                NormalizeId(detail.EntityId).HasValue ||
                NormalizeId(detail.DepartmentId).HasValue ||
                NormalizeId(detail.EmployeeId).HasValue))
            {
                throw new InvalidOperationException("Select at least one Shared With Detail.");
            }

            foreach (var daySchedules in details
                .Where(detail => detail.DayId > 0 && detail.StartDate.HasValue && detail.EndDate.HasValue)
                .GroupBy(detail => detail.DayId))
            {
                var schedules = daySchedules.OrderBy(detail => detail.StartDate).ToList();
                for (var i = 0; i < schedules.Count; i++)
                {
                    var currentSchedule = schedules[i];
                    if (schedules.Take(i).Any(previousSchedule =>
                        RangesOverlap(
                            previousSchedule.StartDate!.Value.Date,
                            previousSchedule.EndDate!.Value.Date,
                            currentSchedule.StartDate!.Value.Date,
                            currentSchedule.EndDate!.Value.Date)))
                    {
                        throw new InvalidOperationException("Schedule date range overlaps another schedule for the same weekday.");
                    }
                }
            }
        }

        private static bool RangesOverlap(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
            => firstStart <= secondEnd && secondStart <= firstEnd;
    }
}
