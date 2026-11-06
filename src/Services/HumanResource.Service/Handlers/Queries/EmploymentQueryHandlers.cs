using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class EmploymentQueryHandlers :
        IRequestHandler<GetEmploymentsV3Query, PaginationResponse<EmploymentV3Dto>>,
        IRequestHandler<GetEmploymentByIdV3Query, EmploymentV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public EmploymentQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<EmploymentV3Dto>> Handle(GetEmploymentsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.EmployeeEmployments
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .AsNoTracking();

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            if (request.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.StartDate ?? DateTime.MinValue)
                .ThenBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .GroupJoin(
                    _context.EmploymentTypes,
                    employment => employment.EmploymentTypeId,
                    type => type.Id,
                    (employment, typeGroup) => new { employment, typeGroup }
                )
                .SelectMany(
                    x => x.typeGroup.DefaultIfEmpty(),
                    (x, type) => new EmploymentV3Dto(
                        x.employment.Id,
                        x.employment.EmployeeId,
                        x.employment.EmploymentTypeId,
                        type != null ? (type.Stxt ?? string.Empty) : string.Empty,
                        x.employment.StartDate,
                        x.employment.EndDate,
                        !x.employment.UtBlock
                    )
                )
                .ToListAsync(cancellationToken);

            return new PaginationResponse<EmploymentV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<EmploymentV3Dto?> Handle(GetEmploymentByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.EmployeeEmployments
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && !x.UtBlock)
                .Where(x => !request.EntityId.HasValue || x.EntityId == request.EntityId.Value)
                .GroupJoin(
                    _context.EmploymentTypes,
                    employment => employment.EmploymentTypeId,
                    type => type.Id,
                    (employment, typeGroup) => new { employment, typeGroup }
                )
                .SelectMany(
                    x => x.typeGroup.DefaultIfEmpty(),
                    (x, type) => new EmploymentV3Dto(
                        x.employment.Id,
                        x.employment.EmployeeId,
                        x.employment.EmploymentTypeId,
                        type != null ? (type.Stxt ?? string.Empty) : string.Empty,
                        x.employment.StartDate,
                        x.employment.EndDate,
                        !x.employment.UtBlock
                    )
                )
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
