using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class ShiftQueryHandlers :
        IRequestHandler<GetShiftsV3Query, PaginationResponse<ShiftV3Dto>>,
        IRequestHandler<GetShiftByIdV3Query, ShiftHeaderV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public ShiftQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<ShiftV3Dto>> Handle(GetShiftsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.Shifts
                .Where(x => x.ClientId == request.ClientId)
                .AsNoTracking();

            var employeeDepartmentId = request.EmployeeId.HasValue
                ? await _context.EmployeeProfiles.AsNoTracking()
                    .Where(x => x.Id == request.EmployeeId.Value
                        && x.ClientId == request.ClientId)
                    .Select(x => x.DepartmentId)
                    .FirstOrDefaultAsync(cancellationToken)
                : null;

            if (request.EntityId.HasValue || request.EmployeeId.HasValue || employeeDepartmentId.HasValue)
            {
                query = query.Where(x =>
                    _context.ShiftSharedDetails.Any(shared =>
                        shared.ShiftId == x.Id &&
                        shared.PClientId == request.ClientId &&
                        ((shared.PDepartId.HasValue &&
                          (!request.EmployeeId.HasValue ||
                           (employeeDepartmentId.HasValue && shared.PDepartId == employeeDepartmentId.Value))) ||
                         (request.EntityId.HasValue && shared.PEntityId == request.EntityId.Value) ||
                         (request.EntityId.HasValue &&
                          shared.PEmpId.HasValue &&
                          _context.EmployeeProfiles.Any(employee =>
                              employee.Id == shared.PEmpId.Value &&
                              employee.ClientId == request.ClientId &&
                              employee.EntityId == request.EntityId.Value)) ||
                         (request.EmployeeId.HasValue && shared.PEmpId == request.EmployeeId.Value) ||
                         (employeeDepartmentId.HasValue && shared.PDepartId == employeeDepartmentId.Value))) ||
                    (!_context.ShiftSharedDetails.Any(shared =>
                        shared.ShiftId == x.Id &&
                        shared.PClientId == request.ClientId) &&
                     request.EntityId.HasValue && x.EntityId == request.EntityId.Value));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x => (x.Stxt != null && x.Stxt.Contains(term)) || (x.Code != null && x.Code.Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var shifts = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // In-Memory Mapping for Lovs
            var shiftTypeIds = shifts.Where(x => x.ShiftTypeId != null).Select(x => x.ShiftTypeId).Distinct().ToList();
            var sharedIds = shifts.Where(x => x.LovSharedId != null).Select(x => x.LovSharedId).Distinct().ToList();
            
            var allLovIds = shiftTypeIds.Concat(sharedIds).Distinct().ToList();

            var lovLookup = new Dictionary<int, string>();
            if (allLovIds.Any())
            {
                lovLookup = await _context.Lovs
                    .Where(l => allLovIds.Contains(l.Id))
                    .Select(l => new { l.Id, l.Stxt })
                    .ToDictionaryAsync(k => k.Id, v => v.Stxt ?? string.Empty, cancellationToken);
            }

            var items = shifts.Select(s => new ShiftV3Dto(
                s.Id,
                s.Code ?? string.Empty,
                s.Stxt ?? string.Empty,
                s.ShiftTypeId,
                s.ShiftTypeId.HasValue && lovLookup.TryGetValue(s.ShiftTypeId.Value, out var shiftTypeStr) ? shiftTypeStr : string.Empty,
                s.LovSharedId,
                s.LovSharedId.HasValue && lovLookup.TryGetValue(s.LovSharedId.Value, out var sharedStr) ? sharedStr : string.Empty,
                true
            )).ToList();

            return new PaginationResponse<ShiftV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<ShiftHeaderV3Dto?> Handle(GetShiftByIdV3Query request, CancellationToken cancellationToken)
        {
            var header = await _context.Shifts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == request.ClientId, cancellationToken);

            if (header == null)
            {
                return null;
            }

            var employeeDepartmentId = request.EmployeeId.HasValue
                ? await _context.EmployeeProfiles.AsNoTracking()
                    .Where(x => x.Id == request.EmployeeId.Value
                        && x.ClientId == request.ClientId)
                    .Select(x => x.DepartmentId)
                    .FirstOrDefaultAsync(cancellationToken)
                : null;

            var hasSharedTargets = await _context.ShiftSharedDetails.AsNoTracking().AnyAsync(shared =>
                shared.ShiftId == header.Id &&
                shared.PClientId == request.ClientId, cancellationToken);
            var isSharedToCurrentScope = await _context.ShiftSharedDetails.AsNoTracking().AnyAsync(shared =>
                    shared.ShiftId == header.Id &&
                    shared.PClientId == request.ClientId &&
                    ((shared.PDepartId.HasValue &&
                      (!request.EmployeeId.HasValue ||
                       (employeeDepartmentId.HasValue && shared.PDepartId == employeeDepartmentId.Value))) ||
                     (request.EntityId.HasValue && shared.PEntityId == request.EntityId.Value) ||
                     (request.EntityId.HasValue &&
                      shared.PEmpId.HasValue &&
                      _context.EmployeeProfiles.Any(employee =>
                          employee.Id == shared.PEmpId.Value &&
                          employee.ClientId == request.ClientId &&
                          employee.EntityId == request.EntityId.Value)) ||
                     (request.EmployeeId.HasValue && shared.PEmpId == request.EmployeeId.Value) ||
                     (employeeDepartmentId.HasValue && shared.PDepartId == employeeDepartmentId.Value)), cancellationToken);
            var isVisibleInScope = (!request.EntityId.HasValue && !request.EmployeeId.HasValue) ||
                isSharedToCurrentScope ||
                (!hasSharedTargets && request.EntityId.HasValue && header.EntityId == request.EntityId.Value);
            if (!isVisibleInScope)
            {
                return null;
            }

            var shiftTypeName = await _context.Lovs
                .Where(x => x.Id == header.ShiftTypeId)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            var sharedName = await _context.Lovs
                .Where(x => x.Id == header.LovSharedId)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            var details = await _context.ShiftDetails
                .AsNoTracking()
                .Where(x => x.ShiftId == header.Id)
                .OrderBy(x => x.DayId)
                .ThenBy(x => x.LineIndex)
                .ToListAsync(cancellationToken);

            var detailIds = details.Select(x => x.Id).ToList();

            var breaks = await _context.ShiftSubDetails
                .AsNoTracking()
                .Where(x => detailIds.Contains(x.ShiftDtlId ?? 0))
                .ToListAsync(cancellationToken);

            var breakLookup = breaks
                .GroupBy(x => x.ShiftDtlId ?? 0)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(b => b.LineIndex).Select(b => new ShiftBreakV3Dto(
                        b.Id,
                        b.LineIndex ?? 0,
                        b.Stxt,
                        b.StartTime,
                        b.EndTime,
                        b.Duration
                    )).ToList());

            var detailDtos = details.Select(d => new ShiftDetailV3Dto(
                d.Id,
                d.LineIndex ?? 0,
                d.DayId ?? 0,
                d.InTime,
                d.OutTime,
                d.StartDate,
                d.EndDate,
                d.GraceTime,
                d.GraceTimeEnd,
                d.LatePolicyApplicable ?? false,
                d.EarlyPolicyApplicable ?? false,
                d.Duration,
                breakLookup.TryGetValue(d.Id, out var list) ? list : new List<ShiftBreakV3Dto>()
            )).ToList();

            var sharedDetails = await _context.ShiftSharedDetails
                .AsNoTracking()
                .Where(x => x.ShiftId == header.Id)
                .OrderBy(x => x.LineIndex)
                .Select(x => new ShiftSharedWithDetailV3Dto(
                    x.Id,
                    x.LineIndex ?? 0,
                    x.LovSharedId,
                    x.PEntityId,
                    x.PDepartId,
                    x.PEmpId
                ))
                .ToListAsync(cancellationToken);

            return new ShiftHeaderV3Dto(
                header.Id,
                header.Code ?? string.Empty,
                header.Stxt ?? string.Empty,
                header.ShiftTypeId,
                shiftTypeName,
                header.LovSharedId,
                sharedName,
                header.WoMonday ?? false,
                header.WoTuesday ?? false,
                header.WoWednesday ?? false,
                header.WoThuresday ?? false,
                header.WoFriday ?? false,
                header.WoSaturday ?? false,
                header.WoSunday ?? false,
                header.LateArrivPolicyId,
                header.EarlyDepPolicyId,
                header.OtPolicyId,
                header.AbsPolicyId,
                header.LateArrivPolicyMnt,
                header.EarlyDepPolicyMnt,
                header.OtPolicyMnt,
                true,
                detailDtos,
                sharedDetails
            );
        }
    }
}
