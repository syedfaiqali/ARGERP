using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;
using System.Text.Json;

namespace Organization.Service.Handlers.Commands
{
    public sealed class InstitutionV3CommandHandlers :
        IRequestHandler<CreateInstitutionV3Command, InstitutionV3Dto>,
        IRequestHandler<UpdateInstitutionV3Command, bool>,
        IRequestHandler<DeleteInstitutionV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public InstitutionV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<InstitutionV3Dto> Handle(CreateInstitutionV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            await ValidateRequest(request, cancellationToken);
            var isSubCampusEntity = await IsSubCampusEntityType(request.EntityTypeId, cancellationToken);

            var code = request.Code?.Trim();
            var instituteName = request.InstituteName?.Trim();
            var nextId = await GetNextId(_context.OrgLegalentityLegacy.IgnoreQueryFilters().Select(x => x.Id), cancellationToken);

            if (await _context.OrgLegalentityLegacy.AnyAsync(
                    x => x.clientid == command.ClientId && !x.utblock && (x.entityno ?? "").Trim().ToUpper() == code.ToUpper(),
                    cancellationToken))
            {
                throw new Exception($"Institution Code: {code} already exists.");
            }

            if (await _context.OrgLegalentityLegacy.AnyAsync(
                    x => x.clientid == command.ClientId && !x.utblock && (x.stxt ?? "").Trim().ToUpper() == instituteName.ToUpper(),
                    cancellationToken))
            {
                throw new Exception($"Institution Name: {instituteName} already exists.");
            }

            var imageId = await SaveInstitutionImageAsync(null, request.LogoImage, command.ClientId, cancellationToken) ?? request.ImageId;
            var watermarkLogo = await SaveInstitutionImageAsync(null, request.WatermarkImage, command.ClientId, cancellationToken) ?? request.WatermarkLogo;

            var entity = new OrgLegalentityLegacy
            {
                Id = nextId,
                entityno = code,
                clientid = command.ClientId,
                stxt = instituteName,
                shortname = request.ShortName?.Trim()?.ToUpperInvariant(),
                estdate = ParseEstDate(request.EstDate),
                contactperson = request.Owner?.Trim(),
                contactno = request.ContactNo?.Trim(),
                address = request.Address?.Trim(),
                countryid = PositiveOrNull(request.CountryId),
                stateid = PositiveOrNull(request.StateId),
                cityid = PositiveOrNull(request.CityId),
                regionid = PositiveOrNull(request.RegionId),
                latitude = request.Latitude,
                longitude = request.Longitude,
                lovorgtypeid = PositiveOrNull(request.InstituteTypeId),
                lovcampustypeid = PositiveOrNull(request.CampusTypeId),
                orgstatusid = PositiveOrNull(request.StatusId),
                registerid = PositiveOrNull(request.AcademicRegisterId),
                slogan = request.Slogan,
                institutiontypeid = PositiveOrNull(request.InstitutionTypeId),
                entitytypeid = PositiveOrNull(request.EntityTypeId),
                headentity = isSubCampusEntity ? PositiveOrNull(request.HeadEntityId) : null,
                imageid = imageId,
                watermarklogo = watermarkLogo,
                utblock = false,
                utsysgen = false,
                utispartial = false,
                utrecordversion = 0
            };


            _context.OrgLegalentityLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            await SaveLocationPolygon(entity.Id, request.LocationPolygon, cancellationToken);
            await ReplaceChildren(entity.Id, request, cancellationToken);

            return new InstitutionV3Dto
            {
                Id = entity.Id,
                Code = entity.entityno,
                InstituteName = entity.stxt,
                ShortName = entity.shortname,
                EstDate = entity.estdate?.ToString("dd-MMM-yyyy"),
                Owner = entity.contactperson,
                ContactNo = entity.contactno,
                Address = entity.address,
                CountryId = entity.countryid,
                StateId = entity.stateid,
                CityId = entity.cityid,
                RegionId = entity.regionid,
                Latitude = entity.latitude,
                Longitude = entity.longitude,
                LocationPolygon = request.LocationPolygon ?? new(),
                EntityTypeId = entity.entitytypeid,
                HeadEntityId = entity.headentity,
                InstitutionTypeId = entity.institutiontypeid,
                InstituteTypeId = entity.lovorgtypeid,
                CampusTypeId = entity.lovcampustypeid,
                StatusId = entity.orgstatusid,
                AcademicRegisterId = entity.registerid,
                Slogan = entity.slogan,
                ImageId = entity.imageid,
                WatermarkLogo = entity.watermarklogo,
                Employees = request.Employees ?? new(),
                Contacts = request.Contacts ?? new(),
                Payments = request.Payments ?? new()
            };
        }

        public async Task<bool> Handle(UpdateInstitutionV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            await ValidateRequest(request, cancellationToken);
            var isSubCampusEntity = await IsSubCampusEntityType(request.EntityTypeId, cancellationToken);

            var entity = await _context.OrgLegalentityLegacy
                .FirstOrDefaultAsync(x => x.clientid == command.ClientId && x.Id == request.Id && !x.utblock, cancellationToken);
            if (entity == null)
            {
                return false;
            }

            var code = request.Code?.Trim();
            var instituteName = request.InstituteName?.Trim();

            if (await _context.OrgLegalentityLegacy.AnyAsync(
                    x => x.clientid == command.ClientId && !x.utblock && x.Id != request.Id && (x.entityno ?? "").Trim().ToUpper() == code.ToUpper(),
                    cancellationToken))
            {
                throw new Exception($"Institution Code: {code} already exists.");
            }

            if (await _context.OrgLegalentityLegacy.AnyAsync(
                    x => x.clientid == command.ClientId && !x.utblock && x.Id != request.Id && (x.stxt ?? "").Trim().ToUpper() == instituteName.ToUpper(),
                    cancellationToken))
            {
                throw new Exception($"Institution Name: {instituteName} already exists.");
            }

            entity.entityno = code;
            entity.utblock = false;
            entity.stxt = instituteName;
            entity.shortname = request.ShortName?.Trim()?.ToUpperInvariant();
            entity.estdate = ParseEstDate(request.EstDate);
            entity.contactperson = request.Owner?.Trim();
            entity.contactno = request.ContactNo?.Trim();
            entity.address = request.Address?.Trim();
            entity.countryid = PositiveOrNull(request.CountryId);
            entity.stateid = PositiveOrNull(request.StateId);
            entity.cityid = PositiveOrNull(request.CityId);
            entity.regionid = PositiveOrNull(request.RegionId);
            entity.latitude = request.Latitude;
            entity.longitude = request.Longitude;
            entity.lovorgtypeid = PositiveOrNull(request.InstituteTypeId);
            entity.lovcampustypeid = PositiveOrNull(request.CampusTypeId);
            entity.orgstatusid = PositiveOrNull(request.StatusId);
            entity.registerid = PositiveOrNull(request.AcademicRegisterId);
            entity.slogan = request.Slogan;
            entity.institutiontypeid = PositiveOrNull(request.InstitutionTypeId);
            entity.entitytypeid = PositiveOrNull(request.EntityTypeId);
            entity.headentity = isSubCampusEntity ? PositiveOrNull(request.HeadEntityId) : null;
            entity.imageid = await SaveInstitutionImageAsync(entity.imageid, request.LogoImage, command.ClientId, cancellationToken) ?? request.ImageId ?? entity.imageid;
            entity.watermarklogo = await SaveInstitutionImageAsync(entity.watermarklogo, request.WatermarkImage, command.ClientId, cancellationToken) ?? request.WatermarkLogo ?? entity.watermarklogo;

            await _context.SaveChangesAsync(cancellationToken);
            await SaveLocationPolygon(entity.Id, request.LocationPolygon, cancellationToken);
            await ReplaceChildren(entity.Id, request, cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteInstitutionV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.OrgLegalentityLegacy
                .FirstOrDefaultAsync(x => x.clientid == request.ClientId && x.Id == request.Id && !x.utblock, cancellationToken);
            if (entity == null)
            {
                return false;
            }

            entity.utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }


        private static string? TrimToNull(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }

        private async Task ReplaceChildren(int entityId, SaveInstitutionV3Request request, CancellationToken cancellationToken)
        {
            var existingContacts = await _context.ORGentitycontactLegacy.Where(x => x.entityid == entityId).ToListAsync(cancellationToken);
            _context.ORGentitycontactLegacy.RemoveRange(existingContacts);

            var existingEmployees = await _context.OrgLegalentitydtlLegacy.Where(x => x.entityid == entityId).ToListAsync(cancellationToken);
            _context.OrgLegalentitydtlLegacy.RemoveRange(existingEmployees);

            var existingPayments = await _context.OrgLegalentitypvdtlLegacy.Where(x => x.entityid == entityId).ToListAsync(cancellationToken);
            _context.OrgLegalentitypvdtlLegacy.RemoveRange(existingPayments);

            var nextContactId = await GetNextId(_context.ORGentitycontactLegacy.IgnoreQueryFilters().Select(x => x.id), cancellationToken);
            var nextEmployeeId = await GetNextId(_context.OrgLegalentitydtlLegacy.IgnoreQueryFilters().Select(x => x.Id), cancellationToken);
            var nextPaymentId = await GetNextId(_context.OrgLegalentitypvdtlLegacy.IgnoreQueryFilters().Select(x => x.Id), cancellationToken);

            var lineIndex = 1;
            foreach (var item in (request.Contacts ?? new()).Where(HasContactData))
            {
                _context.ORGentitycontactLegacy.Add(new ORGentitycontactLegacy
                {
                    id = nextContactId++,
                    entityid = entityId,
                    stxt = item.Name?.Trim(),
                    cntypid = PositiveOrNull(item.ContactTypeId),
                    value = item.Value?.Trim(),
                    lineindex = lineIndex++,
                    utblock = false,
                    utsysgen = false,
                    utispartial = false,
                    utrecordversion = 0
                });
            }

            lineIndex = 1;
            foreach (var item in (request.Employees ?? new()).Where(HasEmployeeData))
            {
                _context.OrgLegalentitydtlLegacy.Add(new OrgLegalentitydtlLegacy
                {
                    Id = nextEmployeeId++,
                    entityid = entityId,
                    lovtypeid = PositiveOrNull(item.TypeId),
                    employeeid = PositiveOrNull(item.EmployeeId),
                    lineindex = lineIndex++,
                    utblock = false,
                    utsysgen = false,
                    utispartial = false,
                    utrecordversion = 0
                });
            }

            lineIndex = 1;
            foreach (var item in (request.Payments ?? new()).Where(HasPaymentData))
            {
                _context.OrgLegalentitypvdtlLegacy.Add(new OrgLegalentitypvdtlLegacy
                {
                    Id = nextPaymentId++,
                    entityid = entityId,
                    pvendorid = PositiveOrNull(item.VendorId),
                    prefix = item.Prefix?.Trim(),
                    paymethodid = PositiveOrNull(item.PayMethodId),
                    accid = PositiveOrNull(item.BankId),
                    isdefault = item.IsDefault,
                    lineindex = lineIndex++,
                    utblock = false,
                    utsysgen = false,
                    utispartial = false,
                    utrecordversion = 0
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<long?> SaveInstitutionImageAsync(long? existingImageId, string? imageDataUri, int clientId, CancellationToken cancellationToken)
        {
            var bytes = DecodeImageBytes(imageDataUri);
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            var image = existingImageId.HasValue
                ? await _context.GNImages.FirstOrDefaultAsync(g => g.Id == existingImageId.Value && g.clientid == clientId, cancellationToken)
                : null;

            if (image == null)
            {
                var nextImageId = (await _context.GNImages.IgnoreQueryFilters().MaxAsync(g => (long?)g.Id, cancellationToken) ?? 0) + 1;
                image = new GNImageLegacy
                {
                    Id = nextImageId,
                    filename = $"{Guid.NewGuid()}.png",
                    img = bytes,
                    clientid = clientId,
                    logdate = DateTime.Now,
                    utsysgen = false,
                    utblock = false,
                    utispartial = false,
                    utrecordversion = 1
                };
                _context.GNImages.Add(image);
            }
            else
            {
                image.img = bytes;
                image.logdate = DateTime.Now;
                image.utrecordversion += 1;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return image.Id;
        }

        private static byte[]? DecodeImageBytes(string? dataUri)
        {
            if (string.IsNullOrWhiteSpace(dataUri))
            {
                return null;
            }

            var payload = dataUri;
            var commaIndex = dataUri.IndexOf(',');
            if (dataUri.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
            {
                payload = dataUri[(commaIndex + 1)..];
            }

            try
            {
                return Convert.FromBase64String(payload);
            }
            catch (FormatException)
            {
                return null;
            }
        }
        private static DateTime? ParseEstDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (DateTime.TryParse(value, out var parsed)) return parsed;
            if (DateTime.TryParseExact(value, "dd-MMM-yyyy", null, System.Globalization.DateTimeStyles.None, out var exact)) return exact;
            return null;
        }

        private static async Task<int> GetNextId(IQueryable<int> ids, CancellationToken cancellationToken)
        {
            return (await ids.MaxAsync(x => (int?)x, cancellationToken) ?? 0) + 1;
        }

        private static int? PositiveOrNull(int? value) => value.GetValueOrDefault() > 0 ? value : null;

        private static bool HasPositiveValue(int? value) => value.GetValueOrDefault() > 0;

        private static bool HasContactData(InstitutionContactV3Dto item) =>
            !string.IsNullOrWhiteSpace(item.Name) ||
            HasPositiveValue(item.ContactTypeId) ||
            !string.IsNullOrWhiteSpace(item.Value);

        private static bool HasEmployeeData(InstitutionEmployeeV3Dto item) =>
            HasPositiveValue(item.TypeId) || HasPositiveValue(item.EmployeeId);

        private static bool HasPaymentData(InstitutionPaymentV3Dto item) =>
            HasPositiveValue(item.VendorId) ||
            HasPositiveValue(item.PayMethodId) ||
            HasPositiveValue(item.BankId) ||
            !string.IsNullOrWhiteSpace(item.Prefix) ||
            item.IsDefault;

        private async Task SaveLocationPolygon(int entityId, List<LocationPointV3Dto>? polygon, CancellationToken cancellationToken)
        {
            if (!await HasLocationPolygonColumn(cancellationToken))
            {
                return;
            }

            var normalized = (polygon ?? new())
                .Where(point => double.IsFinite(point.Latitude) && double.IsFinite(point.Longitude))
                .Select(point => new LocationPointV3Dto
                {
                    Latitude = Math.Round(point.Latitude, 7),
                    Longitude = Math.Round(point.Longitude, 7)
                })
                .ToList();

            var json = normalized.Count == 0 ? null : JsonSerializer.Serialize(normalized);
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE OrgLegalentity SET locationpolygon = {json} WHERE Id = {entityId}",
                cancellationToken);
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
                var result = await command.ExecuteScalarAsync(cancellationToken);
                return Convert.ToInt32(result) == 1;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task ValidateRequest(SaveInstitutionV3Request request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Text(request.Code, "Code", 20);
            OrganizationValidation.Text(request.InstituteName, "Institute Name", 500);
            OrganizationValidation.Text(request.ShortName, "Short Name", 50);
            OrganizationValidation.Text(request.Owner, "Owner", 200);
            OrganizationValidation.Text(request.ContactNo, "Contact No", 50);
            OrganizationValidation.Text(request.Address, "Address", 500);
            OrganizationValidation.Text(request.Slogan, "Slogan", 500);
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new Exception("Code is required.");
            if (!HasPositiveValue(request.EntityTypeId))
                throw new Exception("Entity Type is required.");
            var isCompanyEntity = await IsCompanyEntityType(request.EntityTypeId, cancellationToken);
            var isSubCampusEntity = await IsSubCampusEntityType(request.EntityTypeId, cancellationToken);
            if (string.IsNullOrWhiteSpace(request.ShortName))
                throw new Exception("Short Name is required.");
            if (string.IsNullOrWhiteSpace(request.InstituteName))
                throw new Exception("Institute Name is required.");
            if (!HasPositiveValue(request.StatusId))
                throw new Exception("Status is required.");
            if (!isCompanyEntity && !HasPositiveValue(request.InstitutionTypeId))
                throw new Exception("Institution Type is required.");
            if (!isCompanyEntity && !HasPositiveValue(request.InstituteTypeId))
                throw new Exception("Institute Type is required.");
            if (!isCompanyEntity && !HasPositiveValue(request.CampusTypeId))
                throw new Exception("Campus Type is required.");
            if (isSubCampusEntity && !HasPositiveValue(request.HeadEntityId))
                throw new Exception("Head Campus is required.");
            if (isSubCampusEntity && HasPositiveValue(request.HeadEntityId) && request.Id > 0 && request.HeadEntityId == request.Id)
                throw new Exception("Head Campus cannot be the same institution.");
            if (string.IsNullOrWhiteSpace(request.Owner))
                throw new Exception("Owner is required.");
            if (string.IsNullOrWhiteSpace(request.ContactNo))
                throw new Exception("Contact No is required.");
            if (string.IsNullOrWhiteSpace(request.Address))
                throw new Exception("Address is required.");
            if (!HasPositiveValue(request.CountryId))
                throw new Exception("Country is required.");
            if (!HasPositiveValue(request.StateId))
                throw new Exception("State is required.");
            if (!HasPositiveValue(request.CityId))
                throw new Exception("City is required.");
            if (!HasPositiveValue(request.RegionId))
                throw new Exception("Region is required.");

            var payments = (request.Payments ?? new()).Where(HasPaymentData).ToList();
            var defaults = payments.Count(x => x.IsDefault);
            if (payments.Count > 0 && defaults != 1)
                throw new Exception("Online Payment must contain exactly one default row.");

            if (payments.Any(x => !HasPositiveValue(x.VendorId)))
                throw new Exception("Payment Vendor is required.");
            if (payments.Any(x => !HasPositiveValue(x.PayMethodId)))
                throw new Exception("Pay Method is required.");

            var employees = (request.Employees ?? new()).Where(HasEmployeeData).ToList();
            if (!isCompanyEntity && employees.Any(x => !HasPositiveValue(x.TypeId) || !HasPositiveValue(x.EmployeeId)))
                throw new Exception("Employee Type and Employee are required for each employee row.");
        }

        private async Task<bool> IsCompanyEntityType(int? entityTypeId, CancellationToken cancellationToken)
        {
            if (!HasPositiveValue(entityTypeId))
            {
                return false;
            }

            var entityType = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Id == entityTypeId)
                .Select(x => new { x.code, x.stxt, x.ltxt })
                .FirstOrDefaultAsync(cancellationToken);

            var text = $"{entityType?.code} {entityType?.stxt} {entityType?.ltxt}";
            return text.Contains("company", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> IsSubCampusEntityType(int? entityTypeId, CancellationToken cancellationToken)
        {
            if (!HasPositiveValue(entityTypeId))
            {
                return false;
            }

            var entityType = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Id == entityTypeId)
                .Select(x => new { x.code, x.stxt, x.ltxt })
                .FirstOrDefaultAsync(cancellationToken);

            var text = $"{entityType?.code} {entityType?.stxt} {entityType?.ltxt}";
            var normalized = text.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
            return normalized.Contains("subcampus", StringComparison.OrdinalIgnoreCase);
        }
    }
}
