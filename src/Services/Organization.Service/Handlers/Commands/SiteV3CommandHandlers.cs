using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Handlers;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateSiteV3CommandHandler : IRequestHandler<CreateSiteV3Command, SiteV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateSiteV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<SiteV3Dto> Handle(CreateSiteV3Command request, CancellationToken cancellationToken)
        {
            var clientId = SiteV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = SiteV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var code = SiteV3AccessHelpers.EnsureTrimmed(request.Code, "Code").ToUpperInvariant();
            var siteName = SiteV3AccessHelpers.EnsureTrimmed(request.SiteName, "Site Name");
            var siteTypeId = SiteV3AccessHelpers.EnsurePositive(request.SiteTypeId, "Site Type");
            var siteForId = SiteV3AccessHelpers.EnsurePositive(request.SiteForId, "Site For");
            var regionId = SiteV3AccessHelpers.EnsurePositive(request.RegionId, "Region");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");

            if (siteName.Length > 500)
                throw new InvalidOperationException("Site Name must be 500 characters or less");

            await SiteV3AccessHelpers.ValidateLovAsync(_context, siteTypeId, "STYP", "Site Type", cancellationToken);
            await SiteV3AccessHelpers.ValidateLovAsync(_context, siteForId, "SFOR", "Site For", cancellationToken);

            var regionExists = await _context.OrgRegion
                .AsNoTracking()
                .AnyAsync(r => r.Id == regionId && r.ClientId == clientId, cancellationToken);
            if (!regionExists)
                throw new InvalidOperationException("Region is invalid");

            if (request.ForId.HasValue && request.ForId.Value > 0)
            {
                var parentExists = await _context.OrgSites
                    .AsNoTracking()
                    .AnyAsync(s =>
                        s.Id == request.ForId.Value
                        && s.ClientId == clientId
                        && s.EntityId == entityId
                        , cancellationToken);
                if (!parentExists)
                    throw new InvalidOperationException("Site ID is invalid");
            }

            var duplicateCode = await _context.OrgSites
                .AsNoTracking()
                .AnyAsync(s =>
                    s.ClientId == clientId
                    && s.EntityId == entityId
                    && s.Code != null
                    && (s.Code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (duplicateCode)
                throw new InvalidOperationException("Code already exists");

            var duplicateName = await _context.OrgSites
                .AsNoTracking()
                .AnyAsync(s =>
                    s.ClientId == clientId
                    && s.EntityId == entityId
                    && s.SiteName != null
                    && (s.SiteName ?? "").Trim().ToUpper() == siteName.ToUpper(), cancellationToken);
            if (duplicateName)
                throw new InvalidOperationException("Site Name already exists");

            var nextId = (await _context.OrgSites.IgnoreQueryFilters()
                .MaxAsync(s => (int?)s.Id, cancellationToken) ?? 0) + 1;

            var site = new OrgSite
            {
                Id = nextId,
                Code = code,
                SiteName = siteName,
                SiteTypeId = siteTypeId,
                SiteForId = siteForId,
                RegionId = regionId,
                ForId = request.ForId,
                ParentSiteId = request.ForId,
                EntityId = entityId,
                ClientId = clientId,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0,
            };

            _context.OrgSites.Add(site);
            await _context.SaveChangesAsync(cancellationToken);

            return await SiteV3Mapper.MapByIdAsync(_context, site.Id, clientId, entityId, cancellationToken);
        }
    }

    public sealed class UpdateSiteV3CommandHandler : IRequestHandler<UpdateSiteV3Command, SiteV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateSiteV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<SiteV3Dto> Handle(UpdateSiteV3Command request, CancellationToken cancellationToken)
        {
            var clientId = SiteV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = SiteV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var siteId = SiteV3AccessHelpers.EnsurePositive(request.SiteId, "SiteId");

            var code = SiteV3AccessHelpers.EnsureTrimmed(request.Code, "Code").ToUpperInvariant();
            var siteName = SiteV3AccessHelpers.EnsureTrimmed(request.SiteName, "Site Name");
            var siteTypeId = SiteV3AccessHelpers.EnsurePositive(request.SiteTypeId, "Site Type");
            var siteForId = SiteV3AccessHelpers.EnsurePositive(request.SiteForId, "Site For");
            var regionId = SiteV3AccessHelpers.EnsurePositive(request.RegionId, "Region");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");

            if (siteName.Length > 500)
                throw new InvalidOperationException("Site Name must be 500 characters or less");

            await SiteV3AccessHelpers.ValidateLovAsync(_context, siteTypeId, "STYP", "Site Type", cancellationToken);
            await SiteV3AccessHelpers.ValidateLovAsync(_context, siteForId, "SFOR", "Site For", cancellationToken);

            var regionExists = await _context.OrgRegion
                .AsNoTracking()
                .AnyAsync(r => r.Id == regionId && r.ClientId == clientId, cancellationToken);
            if (!regionExists)
                throw new InvalidOperationException("Region is invalid");

            var site = await _context.OrgSites
                .FirstOrDefaultAsync(s => s.Id == siteId && s.ClientId == clientId && s.EntityId == entityId, cancellationToken);
            if (site == null)
                throw new KeyNotFoundException($"Site with ID {siteId} not found");

            if (request.ForId.HasValue && request.ForId.Value > 0)
            {
                if (request.ForId.Value == siteId)
                    throw new InvalidOperationException("Site ID cannot reference itself");

                var parentExists = await _context.OrgSites
                    .AsNoTracking()
                    .AnyAsync(s =>
                        s.Id == request.ForId.Value
                        && s.ClientId == clientId
                        && s.EntityId == entityId
                        , cancellationToken);
                if (!parentExists)
                    throw new InvalidOperationException("Site ID is invalid");
            }

            var duplicateCode = await _context.OrgSites
                .AsNoTracking()
                .AnyAsync(s =>
                    s.Id != siteId
                    && s.ClientId == clientId
                    && s.EntityId == entityId
                    && s.Code != null
                    && (s.Code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (duplicateCode)
                throw new InvalidOperationException("Code already exists");

            var duplicateName = await _context.OrgSites
                .AsNoTracking()
                .AnyAsync(s =>
                    s.Id != siteId
                    && s.ClientId == clientId
                    && s.EntityId == entityId
                    && s.SiteName != null
                    && (s.SiteName ?? "").Trim().ToUpper() == siteName.ToUpper(), cancellationToken);
            if (duplicateName)
                throw new InvalidOperationException("Site Name already exists");

            site.Code = code;
            site.SiteName = siteName;
            site.SiteTypeId = siteTypeId;
            site.SiteForId = siteForId;
            site.RegionId = regionId;
            site.ForId = request.ForId;
            site.ParentSiteId = request.ForId;
            site.utrecordversion += 1;

            _context.OrgSites.Update(site);
            await _context.SaveChangesAsync(cancellationToken);

            return await SiteV3Mapper.MapByIdAsync(_context, site.Id, clientId, entityId, cancellationToken);
        }
    }

    public sealed class DeleteSiteV3CommandHandler : IRequestHandler<DeleteSiteV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteSiteV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteSiteV3Command request, CancellationToken cancellationToken)
        {
            var clientId = SiteV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = SiteV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var siteId = SiteV3AccessHelpers.EnsurePositive(request.SiteId, "SiteId");

            var site = await _context.OrgSites
                .FirstOrDefaultAsync(s => s.Id == siteId && s.ClientId == clientId && s.EntityId == entityId, cancellationToken);
            if (site == null)
                throw new KeyNotFoundException($"Site with ID {siteId} not found");

            var hasChildren = await _context.OrgSites
                .AnyAsync(s =>
                    s.ParentSiteId == siteId
                    && s.ClientId == clientId
                    && s.EntityId == entityId
                    , cancellationToken);
            if (hasChildren)
                throw new InvalidOperationException("Cannot delete site with child sites");

            site.utblock = true;
            site.utrecordversion += 1;
            _context.OrgSites.Update(site);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    internal static class SiteV3Mapper
    {
        internal static async Task<SiteV3Dto> MapByIdAsync(
            OrganizationDbContext context,
            int siteId,
            int clientId,
            int entityId,
            CancellationToken cancellationToken)
        {
            var query = context.OrgSites
                .AsNoTracking()
                .Where(s => s.Id == siteId && s.ClientId == clientId && s.EntityId == entityId)
                .Select(s => new
                {
                    site = s,
                    regionName = context.OrgRegion
                        .Where(r => r.Id == s.RegionId)
                        .Select(r => r.stxt)
                        .FirstOrDefault(),
                    siteTypeName = context.Lovs
                        .Where(l => l.Id == s.SiteTypeId)
                        .Select(l => l.stxt)
                        .FirstOrDefault(),
                    siteForName = context.Lovs
                        .Where(l => l.Id == s.SiteForId)
                        .Select(l => l.stxt)
                        .FirstOrDefault(),
                    parentName = context.OrgSites
                        .Where(p => p.Id == s.ForId && p.ClientId == clientId && p.EntityId == entityId)
                        .Select(p => p.SiteName)
                        .FirstOrDefault(),
                });

            var model = await query.FirstOrDefaultAsync(cancellationToken);
            if (model == null)
                throw new KeyNotFoundException($"Site with ID {siteId} not found");

            return new SiteV3Dto
            {
                SiteId = model.site.Id,
                Code = model.site.Code ?? string.Empty,
                SiteName = model.site.SiteName ?? string.Empty,
                SiteTypeId = model.site.SiteTypeId ?? 0,
                SiteTypeName = model.siteTypeName ?? string.Empty,
                SiteForId = model.site.SiteForId ?? 0,
                SiteForName = model.siteForName ?? string.Empty,
                RegionId = model.site.RegionId ?? 0,
                RegionName = model.regionName ?? string.Empty,
                ForId = model.site.ForId,
                ForName = model.parentName,
            };
        }
    }
}
