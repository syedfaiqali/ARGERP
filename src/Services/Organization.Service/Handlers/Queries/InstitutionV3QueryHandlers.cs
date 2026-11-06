using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;
using System.Text.Json;

namespace Organization.Service.Handlers.Queries
{
    public sealed class InstitutionV3QueryHandlers :
        IRequestHandler<GetInstitutionsV3Query, PaginationResponse<InstitutionV3Dto>>,
        IRequestHandler<GetInstitutionByIdV3Query, InstitutionV3Dto?>,
        IRequestHandler<GetInstitutionLookupsV3Query, InstitutionLookupsV3Dto>
    {
        private readonly OrganizationDbContext _context;
        private readonly IDbContextFactory<OrganizationDbContext> _contextFactory;

        public InstitutionV3QueryHandlers(
            OrganizationDbContext context,
            IDbContextFactory<OrganizationDbContext> contextFactory)
        {
            _context = context;
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Runs one read query on its own short-lived context, so several can be awaited
        /// together. A single DbContext cannot serve concurrent operations.
        /// </summary>
        private async Task<T> QueryAsync<T>(
            Func<OrganizationDbContext, CancellationToken, Task<T>> query,
            CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            return await query(context, cancellationToken);
        }

        public async Task<PaginationResponse<InstitutionV3Dto>> Handle(GetInstitutionsV3Query request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.OrgLegalentityLegacy
                .Where(x => x.clientid == request.ClientId && !x.utblock)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim();
                query = query.Where(x =>
                    (x.stxt != null && x.stxt.Contains(search)) ||
                    (x.entityno != null && x.entityno.Contains(search)) ||
                    (x.shortname != null && x.shortname.Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            if (request.Lightweight)
            {
                var lightweightItems = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new InstitutionV3Dto
                    {
                        Id = x.Id,
                        Code = x.entityno,
                        InstituteName = x.stxt,
                        ShortName = x.shortname,
                        Address = x.address,
                        InstitutionTypeId = x.institutiontypeid,
                        InstitutionType = _context.Lovs
                            .Where(lov => lov.Id == x.institutiontypeid)
                            .Select(lov => lov.stxt)
                            .FirstOrDefault(),
                        ImageId = x.imageid,
                        WatermarkLogo = x.watermarklogo
                    })
                    .ToListAsync(cancellationToken);

                return new PaginationResponse<InstitutionV3Dto>
                {
                    Items = lightweightItems,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var pageItems = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var ids = pageItems.Select(x => x.Id).ToList();
            var detailMap = await BuildDetailsMap(ids, includeChildDetails: false, cancellationToken);
            var items = pageItems.Select(x => MapEntity(x, detailMap)).ToList();

            return new PaginationResponse<InstitutionV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<InstitutionV3Dto?> Handle(GetInstitutionByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.OrgLegalentityLegacy
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.clientid == request.ClientId && x.Id == request.Id && !x.utblock, cancellationToken);
            if (entity == null)
            {
                return null;
            }

            var detailMap = await BuildDetailsMap(new List<int> { entity.Id }, includeChildDetails: true, cancellationToken);
            return MapEntity(entity, detailMap);
        }

        public async Task<InstitutionLookupsV3Dto> Handle(GetInstitutionLookupsV3Query request, CancellationToken cancellationToken)
        {
            // Kick off the lookups that do not depend on each other; they are awaited together
            // below instead of running one after another.
            var lovsTask = QueryAsync(
                (db, ct) => db.Lovs.AsNoTracking().ToListAsync(ct), cancellationToken);

            var regionsTask = QueryAsync((db, ct) => db.OrgRegion
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .OrderBy(x => x.stxt)
                .Select(x => new LookupItemDto { Id = x.Id, Code = x.Code, Name = x.stxt })
                .ToListAsync(ct), cancellationToken);

            var paymentVendorsTask = QueryAsync((db, ct) => db.PaymentVendorLegacy
                .AsNoTracking()
                .Where(x => !x.utblock)
                .OrderBy(x => x.stxt)
                .Select(x => new LookupItemDto { Id = x.Id, Code = x.code, Name = x.stxt, Description = x.ltxt })
                .ToListAsync(ct), cancellationToken);

            var banksTask = QueryAsync((db, ct) => db.BankAccountSetupLegacy
                .AsNoTracking()
                .Where(x => x.clientid == request.ClientId && !x.utblock)
                .OrderBy(x => x.title)
                .Select(x => new LookupItemDto
                {
                    Id = x.Id,
                    Code = x.code,
                    Name = x.title,
                    Description = x.accountno
                })
                .ToListAsync(ct), cancellationToken);

            var employeesTask = QueryAsync((db, ct) => db.EmployeeinfohdrLegacy
                .AsNoTracking()
                .Where(x => x.clientid == request.ClientId && !x.utblock)
                .OrderBy(x => x.firstname)
                .Select(x => new LookupItemDto
                {
                    Id = x.Id,
                    Name = (x.firstname ?? string.Empty) + " " + (x.lastname ?? string.Empty)
                })
                .ToListAsync(ct), cancellationToken);

            var headCampusesTask = QueryAsync((db, ct) => db.OrgLegalentityLegacy
                .AsNoTracking()
                .Where(x => x.clientid == request.ClientId && !x.utblock)
                .OrderBy(x => x.stxt)
                .Select(x => new LookupItemDto
                {
                    Id = x.Id,
                    Code = x.entityno,
                    Name = x.stxt
                })
                .ToListAsync(ct), cancellationToken);

            var countriesTask = QueryAsync((db, ct) => db.Country
                .AsNoTracking()
                .Where(x => !x.UtBlock)
                .OrderBy(x => x.CountryName)
                .Select(x => new LookupItemDto { Id = x.Id, Code = x.CountryCode, Name = x.CountryName, Description = x.DialCode })
                .ToListAsync(ct), cancellationToken);

            var statesTask = QueryAsync((db, ct) =>
            {
                var q = db.State.AsNoTracking().Where(x => !EF.Property<bool>(x, "utblock"));
                if (request.CountryId.HasValue)
                {
                    q = q.Where(x => x.CountryId == request.CountryId.Value);
                }

                return q
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto { Id = x.Id, Code = x.StateCode, Name = x.stxt })
                    .ToListAsync(ct);
            }, cancellationToken);

            var citiesTask = QueryAsync((db, ct) =>
            {
                var q = db.City.AsNoTracking().Where(x => !x.UtBlock);
                if (request.StateId.HasValue)
                {
                    q = q.Where(x => x.StateId == request.StateId.Value);
                }
                if (request.CountryId.HasValue)
                {
                    q = q.Where(x => x.CountryId == request.CountryId.Value);
                }

                return q
                    .OrderBy(x => x.Citystxt)
                    .Select(x => new LookupItemDto { Id = x.Id, Code = x.CityCode, Name = x.Citystxt, Description = x.DialCode })
                    .ToListAsync(ct);
            }, cancellationToken);

            // Registers depend on the register ids referenced by this client's entities,
            // so that pair stays sequential.
            var registersTask = QueryAsync(async (db, ct) =>
            {
                var usedRegisterIds = await db.OrgLegalentityLegacy
                    .AsNoTracking()
                    .Where(x => x.clientid == request.ClientId && !x.utblock && x.registerid.HasValue)
                    .Select(x => x.registerid!.Value)
                    .Distinct()
                    .ToListAsync(ct);

                return await db.AcademicRegisterLegacy
                    .AsNoTracking()
                    .Where(x => !x.utblock && ((x.clientid == request.ClientId) || usedRegisterIds.Contains(x.Id)))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto { Id = x.Id, Code = x.code, Name = x.stxt, Description = x.ltxt })
                    .ToListAsync(ct);
            }, cancellationToken);

            await Task.WhenAll(
                lovsTask, regionsTask, paymentVendorsTask, banksTask, employeesTask, headCampusesTask,
                countriesTask, statesTask, citiesTask, registersTask);

            var lovs = await lovsTask;
            var regions = await regionsTask;
            var paymentVendors = await paymentVendorsTask;
            var banks = await banksTask;
            var employees = await employeesTask;
            var headCampuses = await headCampusesTask;
            var countries = await countriesTask;
            var states = await statesTask;
            var cities = await citiesTask;
            var registers = await registersTask;

            static List<LookupItemDto> FilterLovs(IEnumerable<Entities.Lov> source, params string[] tags)
            {
                return source
                    .Where(x => tags.Any(t =>
                        (x.vtype ?? string.Empty).Contains(t, StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains(t, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList();
            }

            return new InstitutionLookupsV3Dto
            {
                // Legacy Entity Type is stored in OrgLegalentity.entitytypeid and maps to LOV vtype = OTYP.
                EntityTypes = lovs
                    .Where(x =>
                        string.Equals(x.vtype, "OTYP", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("entity", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("entity", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("orgtype", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("orgtype", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList(),
                HeadCampuses = headCampuses,
                // Legacy Institution Type is sourced from LOV vtype = INSTY.
                InstitutionTypes = lovs
                    .Where(x =>
                        string.Equals(x.vtype, "INSTY", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("institution", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("institution", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList(),
                // Legacy Institute Type is sourced from LOV vtype = ORGT.
                InstituteTypes = lovs
                    .Where(x =>
                        string.Equals(x.vtype, "ORGT", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("orgtype", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("orgtype", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("institute", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("institute", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList(),
                // Legacy Campus Type is stored in OrgLegalentity.lovcampustypeid and maps mostly to LOV vtype = COTY.
                CampusTypes = lovs
                    .Where(x =>
                        string.Equals(x.vtype, "COTY", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(x.vtype, "CPTY", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("campus", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("campus", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList(),
                // Legacy Institution status is stored in OrgLegalentity.orgstatusid and maps to LOV vtype = ORGS.
                Statuses = lovs
                    .Where(x =>
                        string.Equals(x.vtype, "ORGS", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("status", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("status", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList(),
                ContactTypes = FilterLovs(lovs, "cntyp", "contact"),
                // Legacy Institution payment rows (OrgLegalentitypvdtl.paymethodid) reference LOV vtype = CALC (e.g., FIFO/LIFO).
                // Keep old fuzzy tags as fallback for compatibility.
                PayMethods = lovs
                    .Where(x =>
                        string.Equals(x.vtype, "CALC", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("paymethod", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("paymethod", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("calculateon", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("calculateon", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList(),
                AcademicRegisters = registers,
                Regions = regions,
                PaymentVendors = paymentVendors,
                Banks = banks,
                Employees = employees,
                // Legacy employee row type is OrgLegalentitydtl.lovtypeid and maps to LOV vtype = MNG.
                EmployeeTypes = lovs
                    .Where(x =>
                        string.Equals(x.vtype, "MNG", StringComparison.OrdinalIgnoreCase) ||
                        (x.vtype ?? string.Empty).Contains("management", StringComparison.OrdinalIgnoreCase) ||
                        (x.code ?? string.Empty).Contains("management", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.stxt)
                    .Select(x => new LookupItemDto
                    {
                        Id = x.Id,
                        Code = x.code,
                        Name = x.stxt,
                        Description = x.ltxt
                    })
                    .DistinctBy(x => x.Id)
                    .ToList(),
                Countries = countries,
                States = states,
                Cities = cities
            };
        }

        private async Task<InstitutionDetailsMap> BuildDetailsMap(List<int> ids, bool includeChildDetails, CancellationToken cancellationToken)
        {
            var map = new InstitutionDetailsMap();
            if (ids.Count == 0)
            {
                return map;
            }

            var entities = await _context.OrgLegalentityLegacy.AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var countryIds = entities.Where(x => x.countryid.HasValue).Select(x => x.countryid!.Value).Distinct().ToList();
            var stateIds = entities.Where(x => x.stateid.HasValue).Select(x => x.stateid!.Value).Distinct().ToList();
            var cityIds = entities.Where(x => x.cityid.HasValue).Select(x => x.cityid!.Value).Distinct().ToList();
            var regionIds = entities.Where(x => x.regionid.HasValue).Select(x => x.regionid!.Value).Distinct().ToList();
            var headEntityIds = entities.Where(x => x.headentity.HasValue).Select(x => x.headentity!.Value).Distinct().ToList();
            var lovIds = entities
                .SelectMany(x => new int?[] { x.entitytypeid, x.orgstatusid, x.institutiontypeid, x.lovorgtypeid, x.lovcampustypeid })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();
            var registerIds = entities.Where(x => x.registerid.HasValue).Select(x => x.registerid!.Value).Distinct().ToList();
            map.LocationPolygons = await LoadLocationPolygons(ids, cancellationToken);

            map.Countries = countryIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.Country.AsNoTracking()
                    .Where(x => countryIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.CountryName, cancellationToken);

            map.States = stateIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.State.AsNoTracking()
                    .Where(x => stateIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.stxt, cancellationToken);

            map.Cities = cityIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.City.AsNoTracking()
                    .Where(x => cityIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.Citystxt, cancellationToken);

            map.Regions = regionIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.OrgRegion.AsNoTracking()
                    .Where(x => regionIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.stxt, cancellationToken);

            map.HeadCampuses = headEntityIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.OrgLegalentityLegacy.AsNoTracking()
                    .Where(x => headEntityIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.stxt ?? string.Empty, cancellationToken);

            map.Lovs = lovIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.Lovs.AsNoTracking()
                    .Where(x => lovIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.stxt ?? string.Empty, cancellationToken);

            map.Registers = registerIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.AcademicRegisterLegacy.AsNoTracking()
                    .Where(x => registerIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.stxt ?? string.Empty, cancellationToken);

            if (!includeChildDetails)
            {
                return map;
            }

            var contacts = await _context.ORGentitycontactLegacy.AsNoTracking()
                .Where(x => x.entityid.HasValue && ids.Contains(x.entityid.Value) && !x.utblock)
                .OrderBy(x => x.lineindex)
                .ToListAsync(cancellationToken);

            var employees = await _context.OrgLegalentitydtlLegacy.AsNoTracking()
                .Where(x => x.entityid.HasValue && ids.Contains(x.entityid.Value) && !x.utblock)
                .OrderBy(x => x.lineindex)
                .ToListAsync(cancellationToken);

            var payments = await _context.OrgLegalentitypvdtlLegacy.AsNoTracking()
                .Where(x => x.entityid.HasValue && ids.Contains(x.entityid.Value) && !x.utblock)
                .OrderBy(x => x.lineindex)
                .ToListAsync(cancellationToken);

            var employeeIds = employees.Where(x => x.employeeid.HasValue).Select(x => x.employeeid!.Value).Distinct().ToList();
            var vendorIds = payments.Where(x => x.pvendorid.HasValue).Select(x => x.pvendorid!.Value).Distinct().ToList();
            var contactTypeIds = contacts.Where(x => x.cntypid.HasValue).Select(x => x.cntypid!.Value).Distinct().ToList();
            var employeeTypeIds = employees.Where(x => x.lovtypeid.HasValue).Select(x => x.lovtypeid!.Value).Distinct().ToList();
            var payMethodIds = payments.Where(x => x.paymethodid.HasValue).Select(x => x.paymethodid!.Value).Distinct().ToList();
            var bankIds = payments.Where(x => x.accid.HasValue).Select(x => x.accid!.Value).Distinct().ToList();

            var childLovIds = contactTypeIds
                .Concat(employeeTypeIds)
                .Concat(payMethodIds)
                .Distinct()
                .ToList();

            if (childLovIds.Count > 0)
            {
                var childLovs = await _context.Lovs.AsNoTracking()
                    .Where(x => childLovIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.stxt ?? string.Empty, cancellationToken);

                foreach (var kv in childLovs)
                {
                    map.Lovs[kv.Key] = kv.Value;
                }
            }

            map.EmployeeNames = employeeIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.EmployeeinfohdrLegacy.AsNoTracking()
                    .Where(x => employeeIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => ((x.firstname ?? string.Empty) + " " + (x.lastname ?? string.Empty)).Trim(), cancellationToken);

            map.VendorNames = vendorIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.PaymentVendorLegacy.AsNoTracking()
                    .Where(x => vendorIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.stxt ?? string.Empty, cancellationToken);

            map.BankNames = bankIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.BankAccountSetupLegacy.AsNoTracking()
                    .Where(x => bankIds.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(x => x.Id, x => x.title ?? string.Empty, cancellationToken);

            map.ContactsByEntity = contacts.GroupBy(x => x.entityid!.Value).ToDictionary(g => g.Key, g => g.ToList());
            map.EmployeesByEntity = employees.GroupBy(x => x.entityid!.Value).ToDictionary(g => g.Key, g => g.ToList());
            map.PaymentsByEntity = payments.GroupBy(x => x.entityid!.Value).ToDictionary(g => g.Key, g => g.ToList());

            return map;
        }

        private InstitutionV3Dto MapEntity(Entities.OrgLegalentityLegacy entity, InstitutionDetailsMap map)
        {
            var result = new InstitutionV3Dto
            {
                Id = entity.Id,
                Code = entity.entityno,
                EntityTypeId = entity.entitytypeid,
                EntityType = Lookup(map.Lovs, entity.entitytypeid),
                HeadEntityId = entity.headentity,
                HeadCampus = Lookup(map.HeadCampuses, entity.headentity),
                ShortName = entity.shortname,
                EstDate = entity.estdate?.ToString("dd-MMM-yyyy"),
                InstituteName = entity.stxt,
                AcademicRegisterId = entity.registerid,
                AcademicRegister = Lookup(map.Registers, entity.registerid),
                StatusId = entity.orgstatusid,
                Status = Lookup(map.Lovs, entity.orgstatusid),
                InstitutionTypeId = entity.institutiontypeid,
                InstitutionType = Lookup(map.Lovs, entity.institutiontypeid),
                InstituteTypeId = entity.lovorgtypeid,
                InstituteType = Lookup(map.Lovs, entity.lovorgtypeid),
                CampusTypeId = entity.lovcampustypeid,
                CampusType = Lookup(map.Lovs, entity.lovcampustypeid),
                Slogan = entity.slogan,
                Owner = entity.contactperson,
                ContactNo = entity.contactno,
                Address = entity.address,
                CountryId = entity.countryid,
                Country = Lookup(map.Countries, entity.countryid),
                StateId = entity.stateid,
                State = Lookup(map.States, entity.stateid),
                CityId = entity.cityid,
                City = Lookup(map.Cities, entity.cityid),
                RegionId = entity.regionid,
                Region = Lookup(map.Regions, entity.regionid),
                Latitude = entity.latitude,
                Longitude = entity.longitude,
                LocationPolygon = map.LocationPolygons.TryGetValue(entity.Id, out var polygon) ? polygon : new(),
                ImageId = entity.imageid,
                WatermarkLogo = entity.watermarklogo
            };

            if (map.EmployeesByEntity.TryGetValue(entity.Id, out var employeeRows))
            {
                result.Employees = employeeRows.Select(x => new InstitutionEmployeeV3Dto
                {
                    Id = x.Id,
                    TypeId = x.lovtypeid,
                    Type = Lookup(map.Lovs, x.lovtypeid),
                    EmployeeId = x.employeeid,
                    Employee = Lookup(map.EmployeeNames, x.employeeid)
                }).ToList();
            }

            if (map.ContactsByEntity.TryGetValue(entity.Id, out var contactRows))
            {
                result.Contacts = contactRows.Select(x => new InstitutionContactV3Dto
                {
                    Id = x.id,
                    Name = x.stxt,
                    ContactTypeId = x.cntypid,
                    ContactType = Lookup(map.Lovs, x.cntypid),
                    Value = x.value
                }).ToList();
            }

            if (map.PaymentsByEntity.TryGetValue(entity.Id, out var paymentRows))
            {
                result.Payments = paymentRows.Select(x => new InstitutionPaymentV3Dto
                {
                    Id = x.Id,
                    VendorId = x.pvendorid,
                    Vendor = Lookup(map.VendorNames, x.pvendorid),
                    Prefix = x.prefix,
                    PayMethodId = x.paymethodid,
                    PayMethod = Lookup(map.Lovs, x.paymethodid),
                    BankId = x.accid,
                    Bank = Lookup(map.BankNames, x.accid),
                    IsDefault = x.isdefault
                }).ToList();
            }

            return result;
        }

        private static string? Lookup(Dictionary<int, string> dictionary, int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return dictionary.TryGetValue(id.Value, out var value) ? value : null;
        }

        private sealed class InstitutionDetailsMap
        {
            public Dictionary<int, string> Countries { get; set; } = new();
            public Dictionary<int, string> States { get; set; } = new();
            public Dictionary<int, string> Cities { get; set; } = new();
            public Dictionary<int, string> Regions { get; set; } = new();
            public Dictionary<int, string> HeadCampuses { get; set; } = new();
            public Dictionary<int, string> Lovs { get; set; } = new();
            public Dictionary<int, string> Registers { get; set; } = new();
            public Dictionary<int, List<LocationPointV3Dto>> LocationPolygons { get; set; } = new();
            public Dictionary<int, string> EmployeeNames { get; set; } = new();
            public Dictionary<int, string> VendorNames { get; set; } = new();
            public Dictionary<int, string> BankNames { get; set; } = new();
            public Dictionary<int, List<Entities.ORGentitycontactLegacy>> ContactsByEntity { get; set; } = new();
            public Dictionary<int, List<Entities.OrgLegalentitydtlLegacy>> EmployeesByEntity { get; set; } = new();
            public Dictionary<int, List<Entities.OrgLegalentitypvdtlLegacy>> PaymentsByEntity { get; set; } = new();
        }


        private static string? MaskSecret(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : "********";

        private async Task<Dictionary<int, List<LocationPointV3Dto>>> LoadLocationPolygons(List<int> ids, CancellationToken cancellationToken)
        {
            var result = new Dictionary<int, List<LocationPointV3Dto>>();
            if (ids.Count == 0 || !await HasLocationPolygonColumn(cancellationToken))
            {
                return result;
            }

            var idList = string.Join(",", ids.Distinct().Select(id => id.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != System.Data.ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = $"SELECT Id, CAST(locationpolygon AS nvarchar(max)) FROM OrgLegalentity WHERE Id IN ({idList})";
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    if (reader.IsDBNull(1)) continue;
                    var json = reader.GetString(1);
                    if (string.IsNullOrWhiteSpace(json)) continue;
                    try
                    {
                        var points = JsonSerializer.Deserialize<List<LocationPointV3Dto>>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new();
                        result[reader.GetInt32(0)] = points
                            .Where(point => double.IsFinite(point.Latitude) && double.IsFinite(point.Longitude))
                            .Select(point => new LocationPointV3Dto
                            {
                                Latitude = Math.Round(point.Latitude, 7),
                                Longitude = Math.Round(point.Longitude, 7)
                            })
                            .ToList();
                    }
                    catch
                    {
                        result[reader.GetInt32(0)] = new();
                    }
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            return result;
        }

        private async Task<bool> HasLocationPolygonColumn(CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != System.Data.ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT CASE WHEN COL_LENGTH('OrgLegalentity', 'locationpolygon') IS NULL THEN 0 ELSE 1 END";
                var value = await command.ExecuteScalarAsync(cancellationToken);
                return Convert.ToInt32(value) == 1;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }
    }
}
