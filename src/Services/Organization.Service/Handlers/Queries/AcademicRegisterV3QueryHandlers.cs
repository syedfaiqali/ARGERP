using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetAcademicRegistersV3QueryHandler : IRequestHandler<GetAcademicRegistersV3Query, PaginationResponse<AcademicRegisterV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAcademicRegistersV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AcademicRegisterV3Dto>> Handle(GetAcademicRegistersV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.AcademicRegisterLegacy
                .AsNoTracking()
                // Keep the list consistent with update/delete, which only allow
                // active records. Returning blocked rows here let users open a row
                // that would inevitably return 404 on save.
                .Where(x => x.clientid == request.ClientId && !x.utblock)
                .AsQueryable();

            // Academic Registers are client-level setup data. Do not narrow them
            // by campus: a campus without a register configured would otherwise
            // receive an empty list despite valid rows for its client.

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(x => 
                    (x.code != null && x.code.ToLower().Contains(search)) ||
                    (x.stxt != null && x.stxt.ToLower().Contains(search))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AcademicRegisterV3Dto
                {
                    Id = x.Id,
                    Code = x.code ?? string.Empty,
                    RegisterName = x.stxt ?? string.Empty,
                    Description = x.ltxt,
                    ClientId = x.clientid
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<AcademicRegisterV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

    public sealed class GetAcademicRegisterByIdV3QueryHandler : IRequestHandler<GetAcademicRegisterByIdV3Query, AcademicRegisterV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetAcademicRegisterByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicRegisterV3Dto> Handle(GetAcademicRegisterByIdV3Query request, CancellationToken cancellationToken)
        {
            var result = await _context.AcademicRegisterLegacy
                .AsNoTracking()
                .Where(x => x.Id == request.Id && x.clientid == request.ClientId)
                .Select(x => new AcademicRegisterV3Dto
                {
                    Id = x.Id,
                    Code = x.code ?? string.Empty,
                    RegisterName = x.stxt ?? string.Empty,
                    Description = x.ltxt,
                    ClientId = x.clientid
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
                throw new KeyNotFoundException($"Academic Register with ID {request.Id} not found");

            return result;
        }
    }
}
