using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Entities;
using Organization.Service.Services;

namespace Organization.Service.Handlers.Commands
{
    /// <summary>
    /// Shared active-session rules for Academic Years (relabelled "Academic Sessions" for universities).
    ///
    /// Non-university institutions keep the legacy exclusive rule: marking a year current deactivates
    /// every other active year in the same Academic Register. In a multi-year programme, every
    /// distinct semester in a register is active concurrently.
    /// </summary>
    internal static class AcademicYearActiveRules
    {
        // Keep MAX(id) allocation, period replacement and current-year updates
        // in the same transaction so parallel saves cannot allocate the same IDs.
        public static Task<T> ExecuteSaveAsync<T>(OrganizationDbContext context, Func<Task<T>> save, CancellationToken ct) =>
            context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using var transaction = context.Database.IsRelational() && context.Database.CurrentTransaction == null
                    ? await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct) : null;
                try
                {
                    var result = await save();
                    if (transaction != null) await transaction.CommitAsync(ct);
                    return result;
                }
                catch
                {
                    // A transient failure must reload periods and allocate fresh
                    // IDs on retry, rather than reusing failed tracked inserts.
                    context.ChangeTracker.Clear();
                    throw;
                }
            });

        public static bool IsCurrentSession(DateTime? startDate, DateTime? endDate, DateTime today) =>
            startDate?.Date <= today.Date && endDate?.Date >= today.Date;

        /// <summary>
        /// True when the client has any active legal entity whose institution type (LOV vtype INSTY)
        /// normalizes to "university".
        ///
        /// A single client can span several legal entities of differing types (mixed university/school
        /// groups exist in live data), so this deliberately asks "is any of them a university?" rather
        /// than picking one arbitrarily - an unordered FirstOrDefault would let the query plan decide
        /// whether the concurrent-session cap applies.
        /// </summary>
        public static async Task<bool> IsUniversityAsync(
            OrganizationDbContext context,
            int? clientId,
            CancellationToken cancellationToken)
        {
            var typeNames = await (
                from le in context.OrgLegalentityLegacy
                join lov in context.Lovs on le.institutiontypeid equals lov.Id
                where !le.utblock && le.clientid == clientId && le.institutiontypeid != null
                select lov.stxt).Distinct().ToListAsync(cancellationToken);

            return typeNames.Any(x => InstitutionTerminologyService.NormalizeKey(x) == "university");
        }

        // Semester fields depend on the campus being edited, even when the
        // same client also has institutions of a different type.
        public static async Task<bool> IsUniversityCampusAsync(
            OrganizationDbContext context, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            if (entityId is not > 0)
                return false;

            var campus = await context.OrgLegalentityLegacy.AsNoTracking()
                .Where(x => x.Id == entityId && x.clientid == clientId && !x.utblock)
                .Select(x => new { TypeName = context.Lovs.Where(lov => lov.Id == x.institutiontypeid)
                    .Where(lov => !lov.utblock && lov.vtype == "INSTY")
                    .Select(lov => lov.stxt).FirstOrDefault() })
                .FirstOrDefaultAsync(cancellationToken);
            if (campus == null)
                throw new InvalidOperationException("The selected campus is invalid.");
            return InstitutionTerminologyService.NormalizeKey(campus.TypeName) == "university";
        }

        /// <summary>
        /// Applies the active-session rule for a year being saved as current. <paramref name="excludeId"/>
        /// is the row being updated (null on create) so it is not counted against itself.
        /// </summary>
        public static async Task ApplyAsync(
            OrganizationDbContext context,
            int? clientId,
            int? academicRegisterId,
            int? semesterLovId,
            int? excludeId,
            CancellationToken cancellationToken)
        {
            var isMultiYearProgramme = await IsUniversityAsync(context, clientId, cancellationToken);
            var others = await context.AcademicYearLegacy
                .Where(x => !x.utblock
                            && x.clientid == clientId
                            && x.registerid == academicRegisterId
                            && (!isMultiYearProgramme || x.semlovid == semesterLovId)
                            && x.active
                            && (excludeId == null || x.Id != excludeId))
                .ToListAsync(cancellationToken);

            foreach (var other in others)
            {
                other.active = false;
            }
        }
    }

    public sealed class CreateAcademicYearV3CommandHandler : IRequestHandler<CreateAcademicYearV3Command, AcademicYearV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateAcademicYearV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public Task<AcademicYearV3Dto> Handle(CreateAcademicYearV3Command request, CancellationToken cancellationToken) =>
            AcademicYearActiveRules.ExecuteSaveAsync(_context, () => CreateAsync(request, cancellationToken), cancellationToken);

        private async Task<AcademicYearV3Dto> CreateAsync(CreateAcademicYearV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var academicYearName = request.AcademicYear?.Trim();

            if (request.AcademicRegisterId == null || request.AcademicRegisterId <= 0)
                throw new InvalidOperationException("Academic Register is required");

            if (!await _context.AcademicRegisterLegacy.AnyAsync(x =>
                x.Id == request.AcademicRegisterId && x.clientid == request.ClientId, cancellationToken))
                throw new InvalidOperationException("Academic Register is invalid");

            if (string.IsNullOrWhiteSpace(academicYearName))
                throw new InvalidOperationException("Academic Year is required");

            if (request.StartDate == null || request.EndDate == null)
                throw new InvalidOperationException("Start Date and End Date are required");

            if (request.StartDate.Value.Date > request.EndDate.Value.Date)
                throw new InvalidOperationException("Start Date cannot be greater than End Date");

            if (request.SemLovId is > 0 && !await _context.Lovs.AnyAsync(
                    x => x.Id == request.SemLovId.Value && x.vtype == "SEM" && !x.utblock,
                    cancellationToken))
                throw new InvalidOperationException("Selected Semester is invalid");

            var isMultiYearProgramme = await AcademicYearActiveRules.IsUniversityAsync(
                _context, request.ClientId, cancellationToken);
            var isUniversityCampus = await AcademicYearActiveRules.IsUniversityCampusAsync(
                _context, request.ClientId, request.EntityId, cancellationToken);
            if (isUniversityCampus && request.SemLovId is not > 0)
                throw new InvalidOperationException("Semester is required for a multi-year programme.");

            // Duplicate Check
            if (await _context.AcademicYearLegacy.AnyAsync(x => !x.utblock && x.clientid == request.ClientId && (x.stxt ?? "").Trim().ToUpper() == academicYearName.ToUpper() && x.registerid == request.AcademicRegisterId, cancellationToken))
                throw new InvalidOperationException($"Academic Year: {academicYearName} already exists against selected Academic Register");

            if (isUniversityCampus)
            {
                if (await _context.AcademicYearLegacy.AnyAsync(x =>
                        !x.utblock && x.clientid == request.ClientId
                        && x.registerid == request.AcademicRegisterId && x.semlovid == request.SemLovId,
                        cancellationToken))
                    throw new InvalidOperationException("This Semester already exists against the selected Academic Register.");

                var sessionCount = await _context.AcademicYearLegacy.CountAsync(x =>
                    !x.utblock && x.clientid == request.ClientId && x.registerid == request.AcademicRegisterId,
                    cancellationToken);
                if (sessionCount >= 8)
                    throw new InvalidOperationException("A Multi-Year-Programme Academic Register cannot have more than 8 Academic Sessions.");
            }

            // Overlap validation is intentionally disabled to allow parallel/transition year definitions.
            // Legacy data model permits overlapping windows for the same register in some setups.

            if (request.Periods == null || !request.Periods.Any())
                throw new InvalidOperationException("At least one Period is required");

            var nextId = (await _context.AcademicYearLegacy.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var code = $"AY{nextId:D5}";

            var entity = new AcademicYearLegacy
            {
                Id = nextId,
                code = code,
                stxt = academicYearName,
                ltxt = request.Description,
                startdate = request.StartDate,
                enddate = request.EndDate,
                registerid = request.AcademicRegisterId,
                semlovid = request.SemLovId is > 0 ? request.SemLovId : null,
                coursecatid = request.CourseCatId is > 0 ? request.CourseCatId : null,
                active = isMultiYearProgramme
                    ? AcademicYearActiveRules.IsCurrentSession(request.StartDate, request.EndDate, DateTime.Today)
                    : request.CurrentYear,
                sessionnotvisiable = request.SessionNotVisible,
                clientid = request.ClientId,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0
            };

            var nextDetailId = (await _context.AcademicYearDtlLegacy.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            foreach (var p in request.Periods)
            {
                entity.SMAcademicYeardtls.Add(new AcademicYearDtlLegacy
                {
                    Id = nextDetailId++,
                    acadyrid = entity.Id,
                    lineindex = p.LineIndex,
                    year = p.Year,
                    monthid = p.MonthId,
                    periodstxt = p.PeriodName,
                    startdate = p.StartDate,
                    enddate = p.EndDate,
                    utsysgen = false,
                    utblock = false,
                    utispartial = false,
                    utrecordversion = 0
                });
            }

            if (entity.active && !isMultiYearProgramme)
            {
                await AcademicYearActiveRules.ApplyAsync(
                    _context, request.ClientId, request.AcademicRegisterId, request.SemLovId,
                    excludeId: null, cancellationToken);
            }

            _context.AcademicYearLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new AcademicYearV3Dto
            {
                Id = entity.Id,
                Code = entity.code,
                AcademicYear = entity.stxt,
                AcademicRegisterId = entity.registerid,
                SemLovId = entity.semlovid,
                CourseCatId = entity.coursecatid,
                SemLovName = await _context.Lovs.Where(x => x.Id == entity.semlovid).Select(x => x.stxt).FirstOrDefaultAsync(cancellationToken),
                StartDate = entity.startdate,
                EndDate = entity.enddate,
                Description = entity.ltxt,
                SessionNotVisible = entity.sessionnotvisiable,
                CurrentYear = entity.active
            };
        }
    }

    public sealed class UpdateAcademicYearV3CommandHandler : IRequestHandler<UpdateAcademicYearV3Command, AcademicYearV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateAcademicYearV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public Task<AcademicYearV3Dto> Handle(UpdateAcademicYearV3Command request, CancellationToken cancellationToken) =>
            AcademicYearActiveRules.ExecuteSaveAsync(_context, () => UpdateAsync(request, cancellationToken), cancellationToken);

        private async Task<AcademicYearV3Dto> UpdateAsync(UpdateAcademicYearV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.AcademicYearLegacy
                .Include(x => x.SMAcademicYeardtls)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.clientid == request.ClientId && !x.utblock, cancellationToken);
            var academicYearName = request.AcademicYear?.Trim();

            if (entity == null)
                throw new KeyNotFoundException($"Academic Year with ID {request.Id} not found");

            if (request.AcademicRegisterId == null || request.AcademicRegisterId <= 0)
                throw new InvalidOperationException("Academic Register is required");

            if (!await _context.AcademicRegisterLegacy.AnyAsync(x =>
                x.Id == request.AcademicRegisterId && x.clientid == request.ClientId, cancellationToken))
                throw new InvalidOperationException("Academic Register is invalid");

            if (string.IsNullOrWhiteSpace(academicYearName))
                throw new InvalidOperationException("Academic Year is required");

            if (request.StartDate == null || request.EndDate == null)
                throw new InvalidOperationException("Start Date and End Date are required");

            if (request.StartDate.Value.Date > request.EndDate.Value.Date)
                throw new InvalidOperationException("Start Date cannot be greater than End Date");

            if (request.SemLovId is > 0 && !await _context.Lovs.AnyAsync(
                    x => x.Id == request.SemLovId.Value && x.vtype == "SEM" && !x.utblock,
                    cancellationToken))
                throw new InvalidOperationException("Selected Semester is invalid");

            var isMultiYearProgramme = await AcademicYearActiveRules.IsUniversityAsync(
                _context, request.ClientId, cancellationToken);
            var isUniversityCampus = await AcademicYearActiveRules.IsUniversityCampusAsync(
                _context, request.ClientId, request.EntityId, cancellationToken);
            if (isUniversityCampus && request.SemLovId is not > 0)
                throw new InvalidOperationException("Semester is required for a multi-year programme.");

            // Duplicate Check
            if (await _context.AcademicYearLegacy.AnyAsync(x => !x.utblock && x.clientid == request.ClientId && (x.stxt ?? "").Trim().ToUpper() == academicYearName.ToUpper() && x.registerid == request.AcademicRegisterId && x.Id != request.Id, cancellationToken))
                throw new InvalidOperationException($"Academic Year: {academicYearName} already exists against selected Academic Register");

            if (isUniversityCampus && await _context.AcademicYearLegacy.AnyAsync(x =>
                    !x.utblock && x.clientid == request.ClientId && x.registerid == request.AcademicRegisterId
                    && x.semlovid == request.SemLovId && x.Id != request.Id,
                    cancellationToken))
                throw new InvalidOperationException("This semester already exists against the selected Academic Register.");

            // Overlap validation is intentionally disabled to allow parallel/transition year definitions.
            // Legacy data model permits overlapping windows for the same register in some setups.

            if (request.Periods == null || !request.Periods.Any())
                throw new InvalidOperationException("At least one Period is required");

            entity.stxt = academicYearName;
            entity.ltxt = request.Description;
            entity.startdate = request.StartDate;
            entity.enddate = request.EndDate;
            entity.registerid = request.AcademicRegisterId;
            entity.semlovid = request.SemLovId is > 0 ? request.SemLovId : null;
            if (request.CourseCatId.HasValue) entity.coursecatid = request.CourseCatId > 0 ? request.CourseCatId : null;
            entity.active = isMultiYearProgramme
                ? AcademicYearActiveRules.IsCurrentSession(request.StartDate, request.EndDate, DateTime.Today)
                : request.CurrentYear;
            entity.sessionnotvisiable = request.SessionNotVisible;

            if (entity.active && !isMultiYearProgramme)
            {
                await AcademicYearActiveRules.ApplyAsync(
                    _context, request.ClientId, request.AcademicRegisterId, request.SemLovId,
                    excludeId: request.Id, cancellationToken);
            }

            // Retain replaced periods with their creation and deletion audit history.
            foreach (var oldDetail in entity.SMAcademicYeardtls)
                oldDetail.utblock = true;

            var nextDetailId = (await _context.AcademicYearDtlLegacy.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var newDetails = request.Periods.Select(p => new AcademicYearDtlLegacy
            {
                Id = nextDetailId++,
                acadyrid = entity.Id,
                lineindex = p.LineIndex,
                year = p.Year,
                monthid = p.MonthId,
                periodstxt = p.PeriodName,
                startdate = p.StartDate,
                enddate = p.EndDate,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0
            }).ToList();
            _context.AcademicYearDtlLegacy.AddRange(newDetails);

            await _context.SaveChangesAsync(cancellationToken);

            return new AcademicYearV3Dto
            {
                Id = entity.Id,
                Code = entity.code,
                AcademicYear = entity.stxt,
                AcademicRegisterId = entity.registerid,
                SemLovId = entity.semlovid,
                CourseCatId = entity.coursecatid,
                SemLovName = await _context.Lovs.Where(x => x.Id == entity.semlovid).Select(x => x.stxt).FirstOrDefaultAsync(cancellationToken),
                StartDate = entity.startdate,
                EndDate = entity.enddate,
                Description = entity.ltxt,
                SessionNotVisible = entity.sessionnotvisiable,
                CurrentYear = entity.active
            };
        }
    }

    public sealed class DeleteAcademicYearV3CommandHandler : IRequestHandler<DeleteAcademicYearV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteAcademicYearV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAcademicYearV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.AcademicYearLegacy
                .Include(x => x.SMAcademicYeardtls)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.clientid == request.ClientId && !x.utblock, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Academic Year with ID {request.Id} not found");

            entity.utblock = true;
            foreach (var detail in entity.SMAcademicYeardtls)
                detail.utblock = true;

            _context.AcademicYearLegacy.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
