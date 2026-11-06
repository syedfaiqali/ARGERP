using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class ScheduleCommandHandlers :
        IRequestHandler<CreateScheduleV3Command, ScheduleHeaderV3Dto>,
        IRequestHandler<UpdateScheduleV3Command, bool>,
        IRequestHandler<DeleteScheduleV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public ScheduleCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleHeaderV3Dto> Handle(CreateScheduleV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequest(request);
            var entityId = await ResolveEntityIdAsync(command.EntityId, cancellationToken);

            var nameExists = await _context.Schedules
                .AnyAsync(s => s.Stxt == request.Stxt.Trim()
                    && s.ClientId == command.ClientId
                    && (!entityId.HasValue || s.EntityId == entityId.Value),
                    cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Schedule Name already exists.");
            }

            await ValidateDateRangeAvailabilityAsync(
                request.FromDate!.Value,
                request.TillDate!.Value,
                null,
                command.ClientId,
                entityId,
                cancellationToken);

            await ValidateDetailDateAvailabilityAsync(
                request.Details,
                null,
                command.ClientId,
                entityId,
                cancellationToken);

            var nextId = (await _context.Schedules.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var code = string.IsNullOrWhiteSpace(request.Code)
                ? nextId.ToString("0000000000")
                : request.Code.Trim();

            var header = new ScheduleHeaderLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.Stxt.Trim(),
                FromDate = request.FromDate,
                TillDate = request.TillDate,
                ScheduleFor = request.ScheduleForId,
                ShiftId = request.ShiftId,
                AssignTo = request.AssignToId,
                ClientId = command.ClientId,
                EntityId = entityId,
                UtSysGen = false,
                UtIsPartial = false,
                UtRecordVersion = 0,
                UtBlock = false
            };

            _context.Schedules.Add(header);
            await _context.SaveChangesAsync(cancellationToken);

            await SaveDetailsAsync(header.Id, header.ShiftId, command.ClientId, entityId, request.Details, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var handler = new Handlers.Queries.ScheduleQueryHandlers(_context);
            return (await handler.Handle(
                new HumanResource.Service.Queries.GetScheduleByIdV3Query(header.Id, command.ClientId, entityId),
                cancellationToken))!;
        }

        public async Task<bool> Handle(UpdateScheduleV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequest(request);
            var entityId = await ResolveEntityIdAsync(command.EntityId, cancellationToken);

            var header = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == request.Id
                    && s.ClientId == command.ClientId
                    && !(s.UtBlock ?? false)
                    && (!entityId.HasValue || s.EntityId == null || s.EntityId == entityId.Value),
                    cancellationToken);

            if (header == null) return false;

            var nameExists = await _context.Schedules
                .AnyAsync(s => s.Stxt == request.Stxt.Trim()
                    && s.Id != request.Id
                    && s.ClientId == command.ClientId
                    && !(s.UtBlock ?? false)
                    && (!entityId.HasValue || s.EntityId == null || s.EntityId == entityId.Value),
                    cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Schedule Name already exists.");
            }

            await ValidateDateRangeAvailabilityAsync(
                request.FromDate!.Value,
                request.TillDate!.Value,
                request.Id,
                command.ClientId,
                entityId,
                cancellationToken);

            await ValidateDetailDateAvailabilityAsync(
                request.Details,
                request.Id,
                command.ClientId,
                entityId,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                header.Code = request.Code.Trim();
            }

            header.Stxt = request.Stxt.Trim();
            header.FromDate = request.FromDate;
            header.TillDate = request.TillDate;
            header.ScheduleFor = request.ScheduleForId;
            header.ShiftId = request.ShiftId;
            header.AssignTo = request.AssignToId;
            header.UtRecordVersion += 1;
            header.UtSysGen = false;
            header.UtIsPartial = false;

            var existing = await _context.ScheduleDetails
                .Where(d => d.ScheduleId == header.Id)
                .ToListAsync(cancellationToken);
            _context.ScheduleDetails.RemoveRange(existing);

            await SaveDetailsAsync(header.Id, header.ShiftId, command.ClientId, entityId, request.Details, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteScheduleV3Command command, CancellationToken cancellationToken)
        {
            var header = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == command.Id
                    && s.ClientId == command.ClientId
                    && !(s.UtBlock ?? false)
                    && (!command.EntityId.HasValue || s.EntityId == null || s.EntityId == command.EntityId.Value),
                    cancellationToken);

            if (header == null) return false;

            header.UtBlock = true;
            header.UtRecordVersion += 1;
            header.UtSysGen = false;
            header.UtIsPartial = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task ValidateDateRangeAvailabilityAsync(
            DateTime fromDate,
            DateTime tillDate,
            int? excludedScheduleId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var query = _context.Schedules
                .AsNoTracking()
                .Where(s => s.ClientId == clientId
                    && !(s.UtBlock ?? false)
                    && s.FromDate.HasValue
                    && s.TillDate.HasValue
                    && (!excludedScheduleId.HasValue || s.Id != excludedScheduleId.Value));

            if (entityId.HasValue)
            {
                query = query.Where(s => s.EntityId == entityId.Value);
            }

            var normalizedFromDate = fromDate.Date;
            var normalizedTillDate = tillDate.Date;
            var overlappingScheduleExists = await query.AnyAsync(
                s => s.FromDate!.Value.Date <= normalizedTillDate
                    && s.TillDate!.Value.Date >= normalizedFromDate,
                cancellationToken);

            if (overlappingScheduleExists)
            {
                throw new InvalidOperationException(
                    "A schedule already exists for the selected date range.");
            }
        }

        private async Task ValidateDetailDateAvailabilityAsync(
            List<ScheduleDetailV3Dto> details,
            int? excludedScheduleId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var requestedDates = (details ?? new List<ScheduleDetailV3Dto>())
                .Where(d => d.SchDate.HasValue)
                .Select(d => d.SchDate!.Value.Date)
                .Distinct()
                .ToList();

            if (requestedDates.Count == 0) return;

            var query =
                from detail in _context.ScheduleDetails.AsNoTracking()
                join schedule in _context.Schedules.AsNoTracking()
                    on detail.ScheduleId equals schedule.Id
                where !(detail.UtBlock ?? false)
                    && !(schedule.UtBlock ?? false)
                    && schedule.ClientId == clientId
                    && detail.SchDate.HasValue
                    && (!excludedScheduleId.HasValue || schedule.Id != excludedScheduleId.Value)
                select new { detail, schedule };

            if (entityId.HasValue)
            {
                query = query.Where(x => x.schedule.EntityId == entityId.Value);
            }

            var duplicateDateExists = await query.AnyAsync(
                x => requestedDates.Contains(x.detail.SchDate!.Value.Date),
                cancellationToken);

            if (duplicateDateExists)
            {
                throw new InvalidOperationException(
                    "A schedule already exists for one or more selected dates.");
            }
        }

        private async Task<int?> ResolveEntityIdAsync(int? entityId, CancellationToken cancellationToken)
        {
            if (!entityId.HasValue) return null;
            var exists = await _context.OrgLegalentities
                .AsNoTracking()
                .AnyAsync(e => e.Id == entityId.Value && !e.UtBlock, cancellationToken);
            return exists ? entityId : null;
        }

        private async Task SaveDetailsAsync(
            int scheduleId,
            int? headerShiftId,
            int clientId,
            int? entityId,
            List<ScheduleDetailV3Dto> details,
            CancellationToken cancellationToken)
        {
            var list = details ?? new List<ScheduleDetailV3Dto>();
            var nextId = (await _context.ScheduleDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var line = 1;
            var smShiftId = await ResolveSmShiftIdAsync(headerShiftId, clientId, entityId, cancellationToken);

            if (smShiftId == null)
            {
                smShiftId = await CreateSmShiftFromEtShiftAsync(
                    headerShiftId,
                    clientId,
                    entityId,
                    cancellationToken);
            }

            foreach (var d in list)
            {
                var entity = new ScheduleDetailLegacy
                {
                    Id = d.Id ?? nextId++,
                    ScheduleId = scheduleId,
                    LineIndex = d.LineIndex > 0 ? d.LineIndex : line++,
                    SchDate = d.SchDate,
                    Holiday = d.Holiday,
                    ShiftId = smShiftId,
                    TimeIn = d.TimeIn,
                    TimeOut = d.TimeOut,
                    Duration = d.Duration,
                    UtSysGen = false,
                    UtIsPartial = false,
                    UtRecordVersion = 0,
                    UtBlock = false
                };

                _context.ScheduleDetails.Add(entity);
            }
        }

        private async Task<int?> ResolveSmShiftIdAsync(
            int? headerShiftId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (!headerShiftId.HasValue) return null;

            var headerShift = await _context.Shifts.AsNoTracking()
                .Where(s => s.Id == headerShiftId.Value && s.ClientId == clientId)
                .Select(s => new { s.Code, s.Stxt })
                .FirstOrDefaultAsync(cancellationToken);

            if (headerShift == null) return null;

            var code = (headerShift.Code ?? string.Empty).Trim();
            var name = (headerShift.Stxt ?? string.Empty).Trim();

            var smBase = _context.SmShifts.AsNoTracking()
                .Where(s => s.ClientId == clientId && !s.UtBlock);

            var scopes = new List<IQueryable<SmShiftLegacy>>
            {
                entityId.HasValue ? smBase.Where(s => s.EntityId == entityId.Value) : smBase,
                smBase
            };

            foreach (var scope in scopes.Distinct())
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var byCode = await scope
                        .Where(s => (s.Code ?? "").Trim().ToLower() == code.ToLower())
                        .Select(s => (int?)s.Id)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (byCode.HasValue) return byCode;
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var byName = await scope
                        .Where(s => (s.Stxt ?? "").Trim().ToLower() == name.ToLower())
                        .Select(s => (int?)s.Id)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (byName.HasValue) return byName;
                }
            }

            // Last resort: if SM shift id matches ET shift id, use it.
            var byId = await smBase
                .Where(s => s.Id == headerShiftId.Value)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (byId.HasValue) return byId;

            return null;
        }

        private async Task<int?> CreateSmShiftFromEtShiftAsync(
            int? headerShiftId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (!headerShiftId.HasValue) return null;

            var headerShift = await _context.Shifts.AsNoTracking()
                .Where(s => s.Id == headerShiftId.Value && s.ClientId == clientId)
                .Select(s => new { s.Code, s.Stxt })
                .FirstOrDefaultAsync(cancellationToken);

            if (headerShift == null) return null;

            var nextId = (await _context.SmShifts.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var smShift = new SmShiftLegacy
            {
                Id = nextId,
                Code = headerShift.Code,
                Stxt = headerShift.Stxt,
                EntityId = entityId,
                ClientId = clientId,
                UtSysGen = false,
                UtIsPartial = false,
                UtRecordVersion = 0,
                UtBlock = false,
                WoMonday = false,
                WoTuesday = false,
                WoWednesday = false,
                WoThuresday = false,
                WoFriday = false,
                WoSaturday = false,
                WoSunday = false
            };

            _context.SmShifts.Add(smShift);
            await _context.SaveChangesAsync(cancellationToken);
            return smShift.Id;
        }

        private static void ValidateRequest(CreateScheduleV3Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Stxt))
                throw new InvalidOperationException("Schedule Name is required.");
            if (request.FromDate == null)
                throw new InvalidOperationException("From Date is required.");
            if (request.TillDate == null)
                throw new InvalidOperationException("Till Date is required.");
            if (request.ShiftId == null || request.ShiftId <= 0)
                throw new InvalidOperationException("Shift is required.");
            if (request.ScheduleForId == null || request.ScheduleForId <= 0)
                throw new InvalidOperationException("Schedule For is required.");
            if (request.TillDate < request.FromDate)
                throw new InvalidOperationException("Till Date cannot be less than From Date.");
            ValidateDetails(request.Details);
        }

        private static void ValidateRequest(UpdateScheduleV3Request request)
        {
            if (request.Id <= 0)
                throw new InvalidOperationException("Invalid Schedule Id.");
            if (string.IsNullOrWhiteSpace(request.Stxt))
                throw new InvalidOperationException("Schedule Name is required.");
            if (request.FromDate == null)
                throw new InvalidOperationException("From Date is required.");
            if (request.TillDate == null)
                throw new InvalidOperationException("Till Date is required.");
            if (request.ShiftId == null || request.ShiftId <= 0)
                throw new InvalidOperationException("Shift is required.");
            if (request.ScheduleForId == null || request.ScheduleForId <= 0)
                throw new InvalidOperationException("Schedule For is required.");
            if (request.TillDate < request.FromDate)
                throw new InvalidOperationException("Till Date cannot be less than From Date.");
            ValidateDetails(request.Details);
        }

        private static void ValidateDetails(List<ScheduleDetailV3Dto> details)
        {
            if (details == null || details.Count == 0)
                throw new InvalidOperationException("Schedule Detail is required.");

            var duplicateDate = details
                .Where(d => d.SchDate.HasValue)
                .GroupBy(d => d.SchDate!.Value.Date)
                .FirstOrDefault(group => group.Count() > 1);
            if (duplicateDate != null)
            {
                throw new InvalidOperationException(
                    $"Duplicate schedule date {duplicateDate.Key:dd-MMM-yyyy} is not allowed.");
            }

            foreach (var d in details)
            {
                if (d.SchDate == null)
                    throw new InvalidOperationException("Date is required in Schedule Detail.");
                if (d.TimeIn == null)
                    throw new InvalidOperationException("Time In is required in Schedule Detail.");
                if (d.TimeOut == null)
                    throw new InvalidOperationException("Time Out is required in Schedule Detail.");
                if (d.TimeOut != null && d.TimeIn != null && d.TimeOut.Value <= d.TimeIn.Value)
                    throw new InvalidOperationException("Time Out cannot be less than or equal to Time In.");
            }
        }
    }
}
