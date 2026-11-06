using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class FOCalllogV3QueryHandlers :
        IRequestHandler<GetFOCalllogV3Query, PaginationResponse<FOCalllogV3Dto>>,
        IRequestHandler<GetFOCalllogByIdV3Query, FOCalllogV3Dto?>,
        IRequestHandler<GetCallTypeV3Query, List<ComplainTypeLookupDto>>,
        IRequestHandler<GetCallMediumV3Query, List<ComplainTypeLookupDto>>
    {
        private readonly OrganizationDbContext _context;

        public FOCalllogV3QueryHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<FOCalllogV3Dto>> Handle(GetFOCalllogV3Query request, CancellationToken cancellationToken)
        {
            var pageNumber = Math.Max(1, request.PageNumber);
            var pageSize = Math.Clamp(request.PageSize, 1, 200);

            var query = _context.FOCalllogLegacy
                .AsNoTracking()
                .Include(x => x.Purpose)
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock);

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => 
                    (x.Name != null && x.Name.Contains(request.SearchTerm)) ||
                    (x.PhoneNo != null && x.PhoneNo.Contains(request.SearchTerm)) ||
                    (x.LogId != null && x.LogId.Contains(request.SearchTerm)));
            }
            if (request.PurposeTypeId.HasValue)
            {
                query = query.Where(x => x.PurposeTypeId == request.PurposeTypeId);
            }
            if (request.CallTypeId.HasValue)
            {
                query = query.Where(x => x.CallTypeId == request.CallTypeId);
            }
            if (request.CallMediumId.HasValue)
            {
                query = query.Where(x => x.CallMediumId == request.CallMediumId);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CallDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Fetch calltype and callmedium lovs efficiently
            var lovIds = items.SelectMany(x => new[] { x.CallTypeId, x.CallMediumId }).Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
            var lovs = new Dictionary<int, string>();
            if (lovIds.Count > 0)
            {
                lovs = await _context.Lovs
                    .AsNoTracking()
                    .Where(l => lovIds.Contains(l.Id))
                    .ToDictionaryAsync(l => l.Id, l => l.stxt ?? string.Empty, cancellationToken);
            }

            var dtos = items.Select(x => new FOCalllogV3Dto
            {
                Id = x.Id,
                LogId = x.LogId,
                CallDate = x.CallDate?.ToString("yyyy-MM-dd"),
                CallTime = x.CallTime?.ToString("hh:mm tt"),
                PurposeTypeId = x.PurposeTypeId,
                PurposeType = x.Purpose?.Stxt,
                Name = x.Name,
                PhoneNo = x.PhoneNo,
                CallTypeId = x.CallTypeId,
                CallType = x.CallTypeId.HasValue && lovs.ContainsKey(x.CallTypeId.Value) ? lovs[x.CallTypeId.Value] : null,
                CallMediumId = x.CallMediumId,
                CallMedium = x.CallMediumId.HasValue && lovs.ContainsKey(x.CallMediumId.Value) ? lovs[x.CallMediumId.Value] : null,
                Duration = x.Duration,
                Description = x.Description,
                EntityId = x.EntityId
            }).ToList();

            return new PaginationResponse<FOCalllogV3Dto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<FOCalllogV3Dto?> Handle(GetFOCalllogByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.FOCalllogLegacy
                .AsNoTracking()
                .Include(x => x.Purpose)
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id
                      && x.ClientId == request.ClientId
                      && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value)
                      && !x.UtBlock,
                    cancellationToken);

            if (entity == null) return null;

            var lovIds = new List<int>();
            if (entity.CallTypeId.HasValue) lovIds.Add(entity.CallTypeId.Value);
            if (entity.CallMediumId.HasValue) lovIds.Add(entity.CallMediumId.Value);

            var lovs = new Dictionary<int, string>();
            if (lovIds.Count > 0)
            {
                lovs = await _context.Lovs
                    .AsNoTracking()
                    .Where(l => lovIds.Contains(l.Id))
                    .ToDictionaryAsync(l => l.Id, l => l.stxt ?? string.Empty, cancellationToken);
            }

            return new FOCalllogV3Dto
            {
                Id = entity.Id,
                LogId = entity.LogId,
                CallDate = entity.CallDate?.ToString("yyyy-MM-dd"),
                CallTime = entity.CallTime?.ToString("hh:mm tt"),
                PurposeTypeId = entity.PurposeTypeId,
                PurposeType = entity.Purpose?.Stxt,
                Name = entity.Name,
                PhoneNo = entity.PhoneNo,
                CallTypeId = entity.CallTypeId,
                CallType = entity.CallTypeId.HasValue && lovs.ContainsKey(entity.CallTypeId.Value) ? lovs[entity.CallTypeId.Value] : null,
                CallMediumId = entity.CallMediumId,
                CallMedium = entity.CallMediumId.HasValue && lovs.ContainsKey(entity.CallMediumId.Value) ? lovs[entity.CallMediumId.Value] : null,
                Duration = entity.Duration,
                Description = entity.Description,
                EntityId = entity.EntityId
            };
        }

        public async Task<List<ComplainTypeLookupDto>> Handle(GetCallTypeV3Query request, CancellationToken cancellationToken)
        {
            var lovs = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.vtype == "CALLT" && !x.utblock && x.stxt != null)
                .Select(x => new ComplainTypeLookupDto
                {
                    Id = x.Id,
                    Name = x.stxt ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            return lovs;
        }

        public async Task<List<ComplainTypeLookupDto>> Handle(GetCallMediumV3Query request, CancellationToken cancellationToken)
        {
            var lovs = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.vtype == "CALLM" && !x.utblock && x.stxt != null)
                .Select(x => new ComplainTypeLookupDto
                {
                    Id = x.Id,
                    Name = x.stxt ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            return lovs;
        }
    }
}
