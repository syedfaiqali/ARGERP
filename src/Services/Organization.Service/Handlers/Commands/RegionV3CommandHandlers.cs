using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateRegionV3CommandHandler : IRequestHandler<CreateRegionV3Command, RegionV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateRegionV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<RegionV3Dto> Handle(CreateRegionV3Command request, CancellationToken cancellationToken)
        {
            var clientId = EnsureClientId(request.ClientId);

            if (request.CountryId <= 0)
                throw new InvalidOperationException("Country is required");

            var regionCode = request.RegionCode?.Trim();
            var regionName = request.RegionName?.Trim();

            if (string.IsNullOrWhiteSpace(regionCode))
                throw new InvalidOperationException("Region Code is required");

            if (string.IsNullOrWhiteSpace(regionName))
                throw new InvalidOperationException("Region Name is required");

            var normalizedCode = regionCode.ToUpperInvariant();
            if (normalizedCode.Length > 20)
                throw new InvalidOperationException("Region Code must be 20 characters or less");

            if (regionName.Length > 150)
                throw new InvalidOperationException("Region Name must be 150 characters or less");

            var country = await _context.Country
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var codeExists = await _context.OrgRegion
                .AnyAsync(r =>
                    r.ClientId == clientId
                    && !r.UtBlock
                    && (r.Code ?? "").Trim().ToUpper() == normalizedCode.ToUpper(), cancellationToken);

            if (codeExists)
                throw new InvalidOperationException("Region Code already exists");

            var nameExists = await _context.OrgRegion
                .AnyAsync(r =>
                    r.ClientId == clientId
                    && !r.UtBlock
                    && (r.stxt ?? "").Trim().ToUpper() == regionName.ToUpper(), cancellationToken);

            if (nameExists)
                throw new InvalidOperationException("Region Name already exists");

            var nextRegionId = (await _context.OrgRegion.IgnoreQueryFilters()
                .MaxAsync(r => (int?)r.Id, cancellationToken) ?? 0) + 1;

            var region = new OrgRegion
            {
                Id = nextRegionId,
                ClientId = clientId,
                CountryId = request.CountryId,
                Code = normalizedCode,
                stxt = regionName,
                UtSysGen = false,
                UtBlock = false,
                UtIsPartial = false,
                UtRecordVersion = 1,
                //Description = string.Empty,
                //IsActive = request.IsActive,
                //IsDeleted = false,
                //CreatedAt = DateTime.UtcNow,
                //CreatedBy = request.CreatedBy,
            };

            _context.OrgRegion.Add(region);
            await _context.SaveChangesAsync(cancellationToken);

            return new RegionV3Dto
            {
                RegionId = region.Id,
                RegionCode = region.Code,
                RegionName = region.stxt,
                CountryId = region.CountryId ?? 0,
                CountryName = country.CountryName,
                //IsActive = region.IsActive,
            };
        }

        private static int EnsureClientId(int? clientId)
        {
            if (!clientId.HasValue || clientId.Value <= 0)
                throw new InvalidOperationException("X-Client-Id header is required");

            return clientId.Value;
        }
    }

    public sealed class UpdateRegionV3CommandHandler : IRequestHandler<UpdateRegionV3Command, RegionV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateRegionV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<RegionV3Dto> Handle(UpdateRegionV3Command request, CancellationToken cancellationToken)
        {
            var clientId = EnsureClientId(request.ClientId);

            if (request.RegionId <= 0)
                throw new InvalidOperationException("RegionId is required");

            if (request.CountryId <= 0)
                throw new InvalidOperationException("Country is required");

            var regionCode = request.RegionCode?.Trim();
            var regionName = request.RegionName?.Trim();

            if (string.IsNullOrWhiteSpace(regionCode))
                throw new InvalidOperationException("Region Code is required");

            if (string.IsNullOrWhiteSpace(regionName))
                throw new InvalidOperationException("Region Name is required");

            var normalizedCode = regionCode.ToUpperInvariant();
            if (normalizedCode.Length > 20)
                throw new InvalidOperationException("Region Code must be 20 characters or less");

            if (regionName.Length > 150)
                throw new InvalidOperationException("Region Name must be 150 characters or less");

            var region = await _context.OrgRegion
                .FirstOrDefaultAsync(r =>
                    r.Id == request.RegionId
                    && r.ClientId == clientId
                    && !r.UtBlock
                    , cancellationToken);

            if (region == null)
                throw new KeyNotFoundException($"Region with ID {request.RegionId} not found");

            var country = await _context.Country
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var codeExists = await _context.OrgRegion
                .AnyAsync(r =>
                    r.ClientId == clientId
                    && !r.UtBlock
                    && r.Id != request.RegionId
                    && (r.Code ?? "").Trim().ToUpper() == normalizedCode.ToUpper(), cancellationToken);

            if (codeExists)
                throw new InvalidOperationException("Region Code already exists");

            var nameExists = await _context.OrgRegion
                .AnyAsync(r =>
                    r.ClientId == clientId
                    && !r.UtBlock
                    && r.Id != request.RegionId
                    && (r.stxt ?? "").Trim().ToUpper() == regionName.ToUpper(), cancellationToken);

            if (nameExists)
                throw new InvalidOperationException("Region Name already exists");

            region.CountryId = request.CountryId;
            region.Code = normalizedCode;
            region.stxt = regionName;
            region.UtRecordVersion = region.UtRecordVersion <= 0 ? 1 : region.UtRecordVersion + 1;
            //region.IsActive = request.IsActive;
            //region.UpdatedAt = DateTime.UtcNow;
            //region.UpdatedBy = request.UpdatedBy;

            _context.OrgRegion.Update(region);
            await _context.SaveChangesAsync(cancellationToken);

            return new RegionV3Dto
            {
                RegionId = region.Id,
                RegionCode = region.Code,
                RegionName = region.stxt,
                CountryId = region.CountryId ?? 0,
                CountryName = country.CountryName,
                //IsActive = region.IsActive,
            };
        }

        private static int EnsureClientId(int? clientId)
        {
            if (!clientId.HasValue || clientId.Value <= 0)
                throw new InvalidOperationException("X-Client-Id header is required");

            return clientId.Value;
        }
    }

    public sealed class DeleteRegionV3CommandHandler : IRequestHandler<DeleteRegionV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteRegionV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteRegionV3Command request, CancellationToken cancellationToken)
        {
            var clientId = EnsureClientId(request.ClientId);

            if (request.RegionId <= 0)
                throw new InvalidOperationException("RegionId is required");

            var region = await _context.OrgRegion
                .FirstOrDefaultAsync(r =>
                    r.Id == request.RegionId
                    && r.ClientId == clientId
                    && !r.UtBlock
                    , cancellationToken);

            if (region == null)
                throw new KeyNotFoundException($"Region with ID {request.RegionId} not found");

            region.UtBlock = true;
            region.UtRecordVersion = region.UtRecordVersion <= 0 ? 1 : region.UtRecordVersion + 1;

            _context.OrgRegion.Update(region);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static int EnsureClientId(int? clientId)
        {
            if (!clientId.HasValue || clientId.Value <= 0)
                throw new InvalidOperationException("X-Client-Id header is required");

            return clientId.Value;
        }
    }
}
