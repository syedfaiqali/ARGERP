using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class CustomShiftCommandHandlers :
        IRequestHandler<CreateCustomShiftV3Command, CustomShiftHeaderV3Dto>,
        IRequestHandler<UpdateCustomShiftV3Command, bool>,
        IRequestHandler<DeleteCustomShiftV3Command, bool>
    {
        private const int CustomShiftSubDocType = 600106;
        private readonly HumanResourceDbContext _context;

        public CustomShiftCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<CustomShiftHeaderV3Dto> Handle(CreateCustomShiftV3Command command, CancellationToken cancellationToken)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("DbContext not available");
            }

            var request = command.Request;
            ValidateRequest(request);

            var nameExists = await _context.CustomShiftHeaders
                .AnyAsync(x => x.Stxt == request.Stxt.Trim()
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Shift Name already exists.");
            }

            var nextId = (await _context.CustomShiftHeaders.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var code = string.IsNullOrWhiteSpace(request.Code)
                ? nextId.ToString("0000000000")
                : request.Code.Trim();

            var today = DateTime.Now.Date;
            var normalizedStart = NormalizeTime(request.StartTime, today);
            var normalizedEnd = NormalizeTime(request.EndTime, today);

            var effectiveEntityId = request.EntityId ?? command.EntityId;
            if (effectiveEntityId.HasValue)
            {
                var entityExists = await _context.OrgLegalentities.AnyAsync(o => o.Id == effectiveEntityId.Value && o.ClientId == command.ClientId, cancellationToken);
                if (!entityExists) effectiveEntityId = null;
            }


            var entity = new SMCustomShiftHeaderLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.Stxt.Trim(),
                ShiftTypeId = request.ShiftTypeId,
                StartTime = normalizedStart,
                EndTime = normalizedEnd,
                GraceTime = request.GraceTime,
                GraceTimeEd = request.GraceTimeEd,
                WoMonday = request.WoMonday,
                WoTuesday = request.WoTuesday,
                WoWednesday = request.WoWednesday,
                WoThuresday = request.WoThuresday,
                WoFriday = request.WoFriday,
                WoSaturday = request.WoSaturday,
                WoSunday = request.WoSunday,
                ClientId = command.ClientId,
                EntityId = effectiveEntityId,
                UtDocType = null,
                UtSubDocType = CustomShiftSubDocType,
                UtSysGen = false,
                UtIsPartial = false,
                UtRecordVersion = 0,
                UtBlock = !request.IsActive
            };

            _context.CustomShiftHeaders.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            await SaveBreaksAsync(entity.Id, request.Breaks, today, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var handler = new Handlers.Queries.CustomShiftQueryHandlers(_context);
            var result = await handler.Handle(
                new HumanResource.Service.Queries.GetCustomShiftByIdV3Query(entity.Id, command.ClientId, effectiveEntityId),
                cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Created Custom Shift could not be loaded.");
            }

            return result;
        }

        public async Task<bool> Handle(UpdateCustomShiftV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequest(request);

            var entity = await _context.CustomShiftHeaders
                .FirstOrDefaultAsync(x => x.Id == request.Id
                    && x.ClientId == command.ClientId
                    && !x.UtBlock
                    && (!command.EntityId.HasValue || x.EntityId == null || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            var nameExists = await _context.CustomShiftHeaders
                .AnyAsync(x => x.Stxt == request.Stxt.Trim()
                    && x.Id != request.Id
                    && x.ClientId == command.ClientId
                    && !x.UtBlock
                    && (!command.EntityId.HasValue || x.EntityId == null || x.EntityId == command.EntityId.Value), cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Shift Name already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                entity.Code = request.Code.Trim();
            }

            var today = DateTime.Now.Date;
            entity.Stxt = request.Stxt.Trim();
            entity.ShiftTypeId = request.ShiftTypeId;
            entity.StartTime = NormalizeTime(request.StartTime, today);
            entity.EndTime = NormalizeTime(request.EndTime, today);
            entity.GraceTime = request.GraceTime;
            entity.GraceTimeEd = request.GraceTimeEd;
            entity.WoMonday = request.WoMonday;
            entity.WoTuesday = request.WoTuesday;
            entity.WoWednesday = request.WoWednesday;
            entity.WoThuresday = request.WoThuresday;
            entity.WoFriday = request.WoFriday;
            entity.WoSaturday = request.WoSaturday;
            entity.WoSunday = request.WoSunday;
            entity.UtRecordVersion += 1;
            entity.UtSysGen = false;
            entity.UtIsPartial = false;
            entity.UtBlock = !request.IsActive;

            if (request.EntityId.HasValue)
            {
                entity.EntityId = request.EntityId.Value;
            }
            else if (command.EntityId.HasValue)
            {
                entity.EntityId = command.EntityId.Value;
            }


            var existing = await _context.CustomShiftDetails
                .Where(d => d.ShiftId == entity.Id)
                .ToListAsync(cancellationToken);
            _context.CustomShiftDetails.RemoveRange(existing);

            await SaveBreaksAsync(entity.Id, request.Breaks, today, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteCustomShiftV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.CustomShiftHeaders
                .FirstOrDefaultAsync(x => x.Id == command.Id
                    && x.ClientId == command.ClientId
                    && !x.UtBlock
                    && (!command.EntityId.HasValue || x.EntityId == null || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            entity.UtBlock = true;
            entity.UtRecordVersion += 1;
            entity.UtSysGen = false;
            entity.UtIsPartial = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task SaveBreaksAsync(
            int headerId,
            List<CustomShiftBreakV3Dto> breaks,
            DateTime baseDate,
            CancellationToken cancellationToken)
        {
            var list = breaks ?? new List<CustomShiftBreakV3Dto>();
            var nextId = (await _context.CustomShiftDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var line = 1;

            foreach (var br in list)
            {
                var entity = new SMCustomShiftDetailLegacy
                {
                    Id = nextId++,
                    ShiftId = headerId,
                    LineIndex = br.LineIndex > 0 ? br.LineIndex : line++,
                    Stxt = br.Stxt?.Trim(),
                    StartTime = NormalizeTime(br.StartTime, baseDate),
                    EndTime = NormalizeTime(br.EndTime, baseDate),
                    Duration = br.Duration,
                    UtDocType = null,
                    UtSubDocType = CustomShiftSubDocType,
                    UtSysGen = false,
                    UtIsPartial = false,
                    UtRecordVersion = 0,
                    UtBlock = false
                };

                _context.CustomShiftDetails.Add(entity);
            }
        }

        private static DateTime? NormalizeTime(DateTime? value, DateTime baseDate)
        {
            if (!value.HasValue) return null;
            var time = value.Value;
            return new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, time.Hour, time.Minute, time.Second);
        }

        private static void ValidateRequest(CreateCustomShiftV3Request request)
        {
            if (request.ShiftTypeId == null)
                throw new InvalidOperationException("Shift Type is required.");
            if (string.IsNullOrWhiteSpace(request.Stxt))
                throw new InvalidOperationException("Shift Name is required.");
            if (request.StartTime == null)
                throw new InvalidOperationException("Start Time is required.");
            if (request.EndTime == null)
                throw new InvalidOperationException("End Time is required.");
            if (request.EndTime <= request.StartTime)
                throw new InvalidOperationException("End Time cannot be less than or equal to Start Time.");
            if (!request.GraceTime.HasValue || request.GraceTime.Value <= 0)
                throw new InvalidOperationException("Grace Time (Late) must be greater than zero.");
            if (!request.GraceTimeEd.HasValue || request.GraceTimeEd.Value <= 0)
                throw new InvalidOperationException("Grace Time (Early Departure) must be greater than zero.");
            ValidateBreaks(request.Breaks);
        }

        private static void ValidateRequest(UpdateCustomShiftV3Request request)
        {
            if (request.Id <= 0)
                throw new InvalidOperationException("Invalid Custom Shift Id.");
            if (request.ShiftTypeId == null)
                throw new InvalidOperationException("Shift Type is required.");
            if (string.IsNullOrWhiteSpace(request.Stxt))
                throw new InvalidOperationException("Shift Name is required.");
            if (request.StartTime == null)
                throw new InvalidOperationException("Start Time is required.");
            if (request.EndTime == null)
                throw new InvalidOperationException("End Time is required.");
            if (request.EndTime <= request.StartTime)
                throw new InvalidOperationException("End Time cannot be less than or equal to Start Time.");
            if (!request.GraceTime.HasValue || request.GraceTime.Value <= 0)
                throw new InvalidOperationException("Grace Time (Late) must be greater than zero.");
            if (!request.GraceTimeEd.HasValue || request.GraceTimeEd.Value <= 0)
                throw new InvalidOperationException("Grace Time (Early Departure) must be greater than zero.");
            ValidateBreaks(request.Breaks);
        }

        private static void ValidateBreaks(List<CustomShiftBreakV3Dto> breaks)
        {
            if (breaks == null || breaks.Count == 0)
                throw new InvalidOperationException("Break Schedule is required.");

            var normalized = new List<(string Name, DateTime Start, DateTime End, int Line)>();
            var today = DateTime.Now.Date;

            for (var i = 0; i < breaks.Count; i++)
            {
                var br = breaks[i];
                var lineLabel = $"Line {(br.LineIndex > 0 ? br.LineIndex : i + 1)}";

                if (string.IsNullOrWhiteSpace(br.Stxt))
                    throw new InvalidOperationException($"{lineLabel} Break Name is required.");
                if (br.StartTime == null)
                    throw new InvalidOperationException($"{lineLabel} Start Time is required.");
                if (br.EndTime == null)
                    throw new InvalidOperationException($"{lineLabel} End Time is required.");

                var start = NormalizeTime(br.StartTime, today)!.Value;
                var end = NormalizeTime(br.EndTime, today)!.Value;

                if (end < start)
                    throw new InvalidOperationException($"{lineLabel} End Time cannot be less than Start Time.");

                var name = br.Stxt.Trim();
                if (normalized.Any(n => string.Equals(n.Name, name, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException($"Break Name {lineLabel} already exists.");

                var overlap = normalized.FirstOrDefault(n =>
                    (start >= n.Start && start <= n.End) ||
                    (end >= n.Start && end <= n.End));

                if (overlap.Start != default && overlap.End != default)
                    throw new InvalidOperationException($"{lineLabel} Start Time and End Time already exist.");

                normalized.Add((name, start, end, br.LineIndex > 0 ? br.LineIndex : i + 1));
            }
        }
    }
}
