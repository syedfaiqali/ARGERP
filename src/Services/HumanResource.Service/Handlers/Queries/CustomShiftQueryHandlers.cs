using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using HumanResource.Service.Entities;
using MediatR;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class CustomShiftQueryHandlers :
        IRequestHandler<GetCustomShiftsV3Query, PaginationResponse<CustomShiftV3Dto>>,
        IRequestHandler<GetCustomShiftByIdV3Query, CustomShiftHeaderV3Dto?>,
        IRequestHandler<GetCustomShiftsByEntityV3Query, List<CustomShiftV3Dto>>
    {
        private readonly HumanResourceDbContext _context;

        public CustomShiftQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<CustomShiftV3Dto>> Handle(GetCustomShiftsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.CustomShiftHeaders
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .AsNoTracking();

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x => (x.Stxt != null && x.Stxt.Contains(term)) || (x.Code != null && x.Code.Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .GroupJoin(_context.Lovs,
                    h => h.ShiftTypeId,
                    l => l.Id,
                    (h, lovs) => new { h, lovs })
                .SelectMany(
                    x => x.lovs.DefaultIfEmpty(),
                    (x, lov) => new { x.h, lov })
                .GroupJoin(_context.OrgLegalentities,
                    x => x.h.EntityId,
                    e => e.Id,
                    (x, entities) => new { x.h, x.lov, entities })
                .SelectMany(
                    x => x.entities.DefaultIfEmpty(),
                    (x, entity) => new CustomShiftV3Dto(
                        x.h.Id,
                        x.h.Code ?? string.Empty,
                        x.h.Stxt ?? string.Empty,
                        x.h.ShiftTypeId,
                        x.lov != null ? x.lov.Stxt : string.Empty,
                        x.h.StartTime,
                        x.h.EndTime,
                        x.h.GraceTime,
                        x.h.GraceTimeEd,
                        x.h.EntityId,
                        entity != null ? entity.Stxt : string.Empty,
                        !x.h.UtBlock
                    ))


                .ToListAsync(cancellationToken);

            return new PaginationResponse<CustomShiftV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<CustomShiftHeaderV3Dto?> Handle(GetCustomShiftByIdV3Query request, CancellationToken cancellationToken)
        {
            var header = await _context.CustomShiftHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id
                    && x.ClientId == request.ClientId
                    && !x.UtBlock
                    && (!request.EntityId.HasValue || x.EntityId == null || x.EntityId == request.EntityId.Value), cancellationToken);

            if (header == null)
            {
                return null;
            }

            var shiftType = await _context.Lovs
                .Where(x => x.Id == header.ShiftTypeId)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            var entityName = await _context.OrgLegalentities
                .Where(x => x.Id == header.EntityId)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);



            var details = await _context.CustomShiftDetails
                .AsNoTracking()
                .Where(x => x.ShiftId == header.Id && !x.UtBlock)
                .OrderBy(x => x.LineIndex)
                .Select(x => new CustomShiftBreakV3Dto(
                    x.Id,
                    x.LineIndex ?? 0,
                    x.Stxt,
                    x.StartTime,
                    x.EndTime,
                    x.Duration
                ))
                .ToListAsync(cancellationToken);

            return new CustomShiftHeaderV3Dto(
                header.Id,
                header.Code ?? string.Empty,
                header.Stxt ?? string.Empty,
                header.ShiftTypeId,
                shiftType,
                header.StartTime,
                header.EndTime,
                header.GraceTime,
                header.GraceTimeEd,
                header.WoMonday,
                header.WoTuesday,
                header.WoWednesday,
                header.WoThuresday,
                header.WoFriday,
                header.WoSaturday,
                header.WoSunday,
                header.EntityId,
                entityName,
                !header.UtBlock,
                details
            );

        }

        public async Task<List<CustomShiftV3Dto>> Handle(GetCustomShiftsByEntityV3Query request, CancellationToken cancellationToken)
        {
            var items = await _context.CustomShiftHeaders
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock && (x.EntityId == request.EntityId || x.EntityId == null))
                .OrderByDescending(x => x.Id)
                .GroupJoin(_context.Lovs,
                    h => h.ShiftTypeId,
                    l => l.Id,
                    (h, lovs) => new { h, lovs })
                .SelectMany(
                    x => x.lovs.DefaultIfEmpty(),
                    (x, lov) => new { x.h, lov })
                .GroupJoin(_context.OrgLegalentities,
                    x => x.h.EntityId,
                    e => e.Id,
                    (x, entities) => new { x.h, x.lov, entities })
                .SelectMany(
                    x => x.entities.DefaultIfEmpty(),
                    (x, entity) => new CustomShiftV3Dto(
                        x.h.Id,
                        x.h.Code ?? string.Empty,
                        x.h.Stxt ?? string.Empty,
                        x.h.ShiftTypeId,
                        x.lov != null ? x.lov.Stxt : string.Empty,
                        x.h.StartTime,
                        x.h.EndTime,
                        x.h.GraceTime,
                        x.h.GraceTimeEd,
                        x.h.EntityId,
                        entity != null ? entity.Stxt : string.Empty,
                        !x.h.UtBlock
                    ))


                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
