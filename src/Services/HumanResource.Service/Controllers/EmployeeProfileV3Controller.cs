using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Services;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/EmployeeProfile/v3")]
    [Route("api/HumanResource/EmployeeProfile/v3")]
    [Authorize]
    public sealed class EmployeeProfileV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;
        private readonly ILogger<EmployeeProfileV3Controller> _logger;
        private readonly string? _userManagementConnectionString;
        private readonly IVacancyFulfillmentService _vacancyFulfillmentService;
        private const int TeacherCategoryId = 269;
        private const int MaxPageSize = 1000;
        // Meeting medium LOVs. SMmeetinghostinfo keeps one row per provider per
        // employee under these, because both share the same credential columns.
        private const int LovTypeZoom = 1065;
        private const int LovTypeJitsi = 1066;
        private const int LovTypeGoogleMeet = 1274;
        private static readonly HashSet<int> ResignedStatusIds = new() { 126, 127 };
        private static readonly DateTime MaxDate = new DateTime(9998, 12, 31);

        public EmployeeProfileV3Controller(
            HumanResourceDbContext context,
            ILogger<EmployeeProfileV3Controller> logger,
            IConfiguration configuration,
            IVacancyFulfillmentService vacancyFulfillmentService)
        {
            _context = context;
            _logger = logger;
            _userManagementConnectionString = configuration.GetConnectionString("UserManagementDB");
            _vacancyFulfillmentService = vacancyFulfillmentService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeProfileV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 1000,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? entityId = null,
            [FromQuery] int? gradeId = null)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();
            var effectiveEntityId = entityId ?? headerEntityId;

            // Callers ask for pageSize=1000 to fill lookup dropdowns; anything beyond that is
            // a mistake that would keep a request (and a pooled connection) busy for minutes.
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 1 : (pageSize > MaxPageSize ? MaxPageSize : pageSize);

            var query = _context.EmployeeProfiles.AsNoTracking()
                .Where(x => x.ClientId == clientId && !(x.UtBlock ?? false));

            if (effectiveEntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == effectiveEntityId.Value);
            }

            if (gradeId.HasValue && gradeId.Value > 0)
            {
                var selectedGradeId = gradeId.Value;
                query = query.Where(employee =>
                    _context.EmployeePositions
                        .Where(position =>
                            position.EmployeeId == employee.Id &&
                            !(position.UtBlock ?? false))
                        .OrderByDescending(position => position.TillDate ?? MaxDate)
                        .ThenByDescending(position => position.Id)
                        .Select(position => position.GradeId)
                        .FirstOrDefault() == selectedGradeId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(x =>
                    (x.EmployeeCode ?? string.Empty).Contains(term) ||
                    (x.FirstName ?? string.Empty).Contains(term) ||
                    (x.LastName ?? string.Empty).Contains(term));
            }

            var totalCount = await query.CountAsync();
            var employees = await query.OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var ids = employees.Select(x => x.Id).ToList();

            var rosters = await _context.EmployeeRosters.AsNoTracking()
                .Where(x => x.EmployeeId != null && ids.Contains(x.EmployeeId.Value) && !(x.UtBlock ?? false))
                .ToListAsync();

            var addresses = await _context.EmployeeAddresses.AsNoTracking()
                .Where(x => x.EmployeeId != null && ids.Contains(x.EmployeeId.Value) && !(x.UtBlock ?? false))
                .ToListAsync();
            var positions = await _context.EmployeePositions.AsNoTracking()
                .Where(x => x.EmployeeId != null && ids.Contains(x.EmployeeId.Value) && !(x.UtBlock ?? false))
                .ToListAsync();

            var subCoordinates = await LoadSubCoordinatesForEmployeesAsync(ids);
            var meetingHosts = await _context.MeetingHostInfos.AsNoTracking()
                .Where(x => x.EmployeeId != null && ids.Contains(x.EmployeeId.Value) && !x.UtBlock)
                .ToListAsync();

            // Group once instead of re-scanning every child list for each employee: on a
            // 1000-row page those scans dominated the request.
            var rostersByEmployee = rosters.Where(x => x.EmployeeId.HasValue).ToLookup(x => x.EmployeeId!.Value);
            var addressesByEmployee = addresses.Where(x => x.EmployeeId.HasValue).ToLookup(x => x.EmployeeId!.Value);
            var positionsByEmployee = positions.Where(x => x.EmployeeId.HasValue).ToLookup(x => x.EmployeeId!.Value);
            var subCoordinatesByEmployee = subCoordinates.Where(x => x.EmployeeId.HasValue).ToLookup(x => x.EmployeeId!.Value);
            var meetingHostsByEmployee = meetingHosts.Where(x => x.EmployeeId.HasValue).ToLookup(x => x.EmployeeId!.Value);

            var items = employees
                .Select(emp => MapToDto(
                    emp,
                    PickRoster(rostersByEmployee[emp.Id], emp.Id),
                    PickAddress(addressesByEmployee[emp.Id], emp.Id),
                    PickSubCoordinate(subCoordinatesByEmployee[emp.Id], emp.Id),
                    PickPosition(positionsByEmployee[emp.Id], emp.Id),
                    PickHost(meetingHostsByEmployee[emp.Id], LovTypeZoom),
                    PickHost(meetingHostsByEmployee[emp.Id], LovTypeJitsi),
                    PickHost(meetingHostsByEmployee[emp.Id], LovTypeGoogleMeet)))
                .ToList();

            return Ok(new PaginationResponse<EmployeeProfileV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeProfileV3Dto>> GetById(int id)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            var employee = await _context.EmployeeProfiles.AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false) &&
                    (!headerEntityId.HasValue || x.EntityId == headerEntityId.Value));

            if (employee == null) return NotFound();

            var roster = await _context.EmployeeRosters.AsNoTracking()
                .Where(x => x.EmployeeId == employee.Id && !(x.UtBlock ?? false))
                .OrderByDescending(x => x.EndDate)
                .FirstOrDefaultAsync();

            var address = await _context.EmployeeAddresses.AsNoTracking()
                .Where(x => x.EmployeeId == employee.Id && !(x.UtBlock ?? false))
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            var subCoordinate = await LoadLatestSubCoordinateAsync(employee.Id);

            // Local copy so EF parameterizes the sentinel. A static readonly field is
            // inlined as a datetime2-precision literal, which the datetime column rejects.
            var maxDate = MaxDate;
            var position = await _context.EmployeePositions.AsNoTracking()
                .Where(x => x.EmployeeId == employee.Id && !(x.UtBlock ?? false))
                .OrderByDescending(x => x.TillDate ?? maxDate).ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            var meetingHostRows = await _context.MeetingHostInfos.AsNoTracking()
                .Where(x => x.EmployeeId == employee.Id && !x.UtBlock)
                .ToListAsync();

            return Ok(MapToDto(employee, roster, address, subCoordinate, position,
                PickHost(meetingHostRows, LovTypeZoom), PickHost(meetingHostRows, LovTypeJitsi),
                    PickHost(meetingHostRows, LovTypeGoogleMeet)));
        }

        // Teaching Specialization is read on its own so the 1000-row list that lookup
        // dropdowns share does not pay for it. It is the employee's subject types in
        // EREmployeesubjectskill, the rows the Subject Rating tab shows, and the classes
        // and Professor Categories in EREmployeeinfohdr.teachingclassids.
        [HttpGet("{id:int}/teaching-subject-types")]
        public async Task<ActionResult<object>> GetTeachingSubjectTypes(int id, CancellationToken cancellationToken)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            var subjectTypeIds = await _context.EmployeeSubjectRatings
                .AsNoTracking()
                .Where(x => x.EmployeeId == id && !x.UtBlock && x.SubjectTypeId != null &&
                    x.Employee != null && x.Employee.ClientId == clientId && !(x.Employee.UtBlock ?? false) &&
                    (!entityId.HasValue || x.Employee.EntityId == entityId.Value))
                .OrderBy(x => x.Id)
                .Select(x => x.SubjectTypeId!.Value)
                .ToListAsync(cancellationToken);
            var specialization = await ReadTeachingSpecializationAsync(id, clientId, entityId, cancellationToken);

            return Ok(new
            {
                teachingSubjectTypeIds = subjectTypeIds.Distinct(),
                teachingClassIds = specialization.ClassIds,
                teachingCourseCategoryIds = specialization.CourseCategoryIds,
                teachingCourseIds = specialization.CourseIds
            });
        }

        [HttpGet("creation-policy")]
        public async Task<ActionResult<EmployeeCreationPolicyV3Dto>> GetCreationPolicy(
            [FromQuery] int? entityId = null,
            CancellationToken cancellationToken = default)
        {
            var effectiveEntityId = NormalizeId(entityId) ?? GetEntityId();
            if (!effectiveEntityId.HasValue)
            {
                return BadRequest("EntityId is required.");
            }

            return Ok(await ResolveEmployeeCreationPolicyAsync(
                GetClientId(),
                effectiveEntityId.Value,
                cancellationToken));
        }

        [HttpGet("next-employee-code")]
        public async Task<ActionResult<object>> GetNextEmployeeCode(CancellationToken cancellationToken)
        {
            var employeeCode = await GenerateEmployeeCodeFromNumberConfigAsync(GetClientId(), cancellationToken);
            return Ok(new { employeeCode });
        }

        [HttpGet("EmployeeBirthdays")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployeeBirthdays([FromQuery] int? entityId = null)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();
            var effectiveEntityId = headerEntityId ?? entityId;
            var today = DateTime.Today;

            var employees = await _context.EmployeeProfiles.AsNoTracking()
                .Where(x =>
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false) &&
                    (!effectiveEntityId.HasValue || x.EntityId == effectiveEntityId.Value) &&
                    x.DateOfBirth.HasValue &&
                    x.DateOfBirth.Value.Month == today.Month &&
                    x.DateOfBirth.Value.Day == today.Day)
                .Select(x => new
                {
                    x.Id,
                    x.EmployeeCode,
                    x.FirstName,
                    x.LastName,
                    x.DateOfBirth,
                    x.DesignationId,
                    x.ImageId
                })
                .ToListAsync();

            var designationIds = employees.Where(x => x.DesignationId.HasValue).Select(x => x.DesignationId!.Value).Distinct().ToList();
            var designationMap = await _context.Designations.AsNoTracking()
                .Where(x => designationIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty);

            var result = employees.Select(x => new
            {
                id = x.Id,
                employeecode = x.EmployeeCode ?? string.Empty,
                firstname = new[] { x.FirstName, x.LastName }.Where(v => !string.IsNullOrWhiteSpace(v)).Any()
                    ? string.Join(" ", new[] { x.FirstName, x.LastName }.Where(v => !string.IsNullOrWhiteSpace(v)))
                    : string.Empty,
                DOB = x.DateOfBirth,
                designationstxt = x.DesignationId.HasValue && designationMap.TryGetValue(x.DesignationId.Value, out var designation) ? designation : string.Empty,
                imageId = x.ImageId,
                age = x.DateOfBirth.HasValue ? today.Year - x.DateOfBirth.Value.Year - (x.DateOfBirth.Value.Date > today.AddYears(-(today.Year - x.DateOfBirth.Value.Year)) ? 1 : 0) : 0
            });

            return Ok(result);
        }

        [HttpGet("defaults")]
        public async Task<ActionResult<EmployeeProfileDefaultsV3Dto>> GetDefaults([FromQuery] int? entityId = null)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();
            var effectiveEntityId = entityId ?? headerEntityId;
            return Ok(await ResolveEmployeeProfileDefaults(clientId, effectiveEntityId));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeProfileV3Dto>> Create([FromBody] CreateEmployeeProfileV3Request request)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();
            var entityId = request.EntityId ?? headerEntityId;
            if (!entityId.HasValue)
            {
                return BadRequest("EntityId is required.");
            }

            var creationPolicy = await ResolveEmployeeCreationPolicyAsync(
                clientId,
                entityId.Value,
                HttpContext.RequestAborted);
            if (!creationPolicy.CanCreateNewEmployee)
            {
                return BadRequest(creationPolicy.Message);
            }

            var defaults = await ResolveEmployeeProfileDefaults(clientId, entityId.Value);
            var birthCountryId = NormalizeId(request.BirthCountryId) ?? defaults.BirthCountryId;
            // Birth City is a required employee-provided value; do not silently
            // replace an omitted selection with the entity's default city.
            var birthCityId = NormalizeId(request.BirthCityId);
            var nationalityId = NormalizeId(request.NationalityId) ?? defaults.NationalityId ?? birthCountryId;

            var teachingSubjectTypes = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingSubjectTypeIds);
            var teachingClasses = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingClassIds);
            var teachingCategories = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingCourseCategoryIds);
            var teachingCourses = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingCourseIds);
            var courseCategoryId = ResolveCourseCategoryId(request.CourseCategoryId, teachingCategories);

            var errors = ValidateRequest(
                request.FirstName,
                request.PositionTypeId,
                request.JoiningDate,
                request.EmployeeTypeId,
                request.EmployeeCategoryId,
                courseCategoryId,
                request.DateOfBirth,
                request.DepartmentId,
                request.DesignationId,
                request.StatusId,
                request.GradeId,
                request.ResignDate,
                request.GenderId,
                birthCountryId,
                birthCityId,
                nationalityId,
                request.CellNo,
                request.ShiftId);

            if (errors.Count > 0)
            {
                return BadRequest(string.Join(" ", errors));
            }
            if (NeedsTeachingClassIdsColumn(teachingClasses, teachingCategories, teachingCourses) && !await TeachingClassIdsColumnExistsAsync())
            {
                return BadRequest(TeachingClassIdsColumnMissingMessage);
            }
            if (!await IsGradeInScope(request.GradeId, clientId))
            {
                return BadRequest("Grade is not valid for the current client.");
            }

            if (!await IsSubCampusInScope(request.SubCampusId, clientId, entityId.Value))
            {
                return BadRequest("Selected sub campus does not belong to the employee entity. Please select a sub campus of the selected campus or choose None.");
            }

            HRVacancyPosition? vacancyPosition = null;
            HRVacancy? linkedVacancy = null;
            if (request.VacancyPositionId.HasValue)
            {
                try
                {
                    vacancyPosition = await _vacancyFulfillmentService.ValidateAvailablePositionAsync(
                        request.VacancyPositionId.Value, clientId, entityId.Value, CancellationToken.None);
                    linkedVacancy = await _context.Vacancies.AsNoTracking()
                        .FirstAsync(x => x.Id == vacancyPosition.VacancyId);
                }
                catch (InvalidOperationException exception)
                {
                    return BadRequest(exception.Message);
                }

                if (request.DepartmentId != linkedVacancy.DepartmentId
                    || request.DesignationId != linkedVacancy.DesignationId
                    || request.GradeId != linkedVacancy.GradeId
                    || (linkedVacancy.EmploymentTypeId.HasValue && request.EmployeeTypeId != linkedVacancy.EmploymentTypeId))
                {
                    return BadRequest("Employee position fields must match the selected vacancy position. Reload the vacancy defaults and try again.");
                }
            }

            var nextId = (await _context.EmployeeProfiles.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
            var reportingToError = await ValidateReportingToEmployeeAsync(
                request.ReportingToEmployeeId,
                nextId,
                clientId,
                entityId.Value);
            if (reportingToError != null)
            {
                return BadRequest(reportingToError);
            }

            var substituteError = await ValidateSubstituteAsync(request.SubstituteEmployeeId, nextId, clientId, entityId.Value);
            if (substituteError != null) return BadRequest(substituteError);

            var employeeCode = await GenerateEmployeeCodeFromNumberConfigAsync(clientId, CancellationToken.None)
                ?? (string.IsNullOrWhiteSpace(request.EmployeeCode)
                    ? nextId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    : request.EmployeeCode.Trim());

            if (await _context.EmployeeProfiles.AnyAsync(x =>
                    x.EmployeeCode == employeeCode &&
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false)))
            {
                return BadRequest($"Employee Code '{employeeCode}' already exists.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var entity = new EmployeeProfileLegacy
            {
                Id = nextId,
                ClientId = clientId,
                EntityId = entityId,
                EmployeeCode = employeeCode,
                MachineCode = request.MachineCode?.Trim(),
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                ParentName = request.ParentName?.Trim(),
                PositionTypeId = request.PositionTypeId,
                EmployeeTypeId = request.EmployeeTypeId,
                EmployeeCategoryId = request.EmployeeCategoryId,
                CourseCategoryId = request.EmployeeCategoryId == TeacherCategoryId ? courseCategoryId : null,
                DepartmentId = request.DepartmentId,
                DesignationId = request.DesignationId,
                ReportingToEmployeeId = NormalizeId(request.ReportingToEmployeeId),
                SubstituteEmployeeId = NormalizeId(request.SubstituteEmployeeId),
                StatusId = request.StatusId,
                GradeId = request.GradeId,
                JoiningDate = request.JoiningDate,
                ResignDate = request.ResignDate,
                DateOfBirth = request.DateOfBirth,
                GenderId = request.GenderId,
                BloodGroupId = request.BloodGroupId,
                MaritalStatusId = request.MaritalStatusId,
                BirthCountryId = birthCountryId,
                BirthCityId = birthCityId,
                NationalityId = nationalityId,
                ReligionId = request.ReligionId,
                EmailId = request.EmailId?.Trim(),
                CellNo = request.CellNo?.Trim(),
                CnicNo = request.CnicNo?.Trim(),
                SessiNo = request.SessiNo?.Trim(),
                EobiNo = request.EobiNo?.Trim(),
                CprNo = request.CprNo?.Trim(),
                CprExpiry = request.CprExpiry,
                PassportNo = request.PassportNo?.Trim(),
                PassportExpiry = request.PassportExpiry,
                VisaNo = request.VisaNo?.Trim(),
                VisaExpiry = request.VisaExpiry,
                MoeId = request.MoeId,
                MoeExpiry = request.MoeExpiry,
                HeaderAddress = request.HeaderAddress?.Trim(),
                DrivingLicenseNo = request.DrivingLicenseNo?.Trim(),
                LicenseExpiry = request.LicenseExpiry,
                EmergencyNo = request.EmergencyNo?.Trim(),
                VehicleId = request.VehicleId,
                HomeStopId = request.HomeStopId,
                TValidFrom = request.TValidFrom,
                TValidTill = request.TValidTill,
                TActive = request.TActive ?? false,
                ShiftId = request.ShiftId,
                ImageId = request.ImageId,
                UtBlock = false
            };

            _context.EmployeeProfiles.Add(entity);
            await _context.SaveChangesAsync();

            // A new employee has no subject skills, classes or categories yet, so only chosen ones need writing.
            if (teachingSubjectTypes.Write && teachingSubjectTypes.Value.Length > 0)
            {
                await SyncTeachingSubjectTypesAsync(entity.Id, clientId, teachingSubjectTypes.Value);
                await _context.SaveChangesAsync();
            }
            if (teachingClasses.Value.Length > 0 || teachingCategories.Value.Length > 0 || teachingCourses.Value.Length > 0)
                await WriteTeachingSpecializationAsync(entity.Id, clientId, teachingClasses, teachingCategories, teachingCourses);

            var meetingHost = await UpsertMeetingHostInfo(entity.Id, entity.ClientId, new MeetingHostCredentials(
                request.ZoomUsername, request.ZoomPassword, request.ZoomMeetingId, request.ZoomMeetingPassword,
                request.ZoomInvitePassword, request.ZoomApiKey, request.ZoomApiSecret, request.ZoomSdkKey,
                request.ZoomSdkSecret, request.ZoomToken, request.ZoomAccountId, request.ZoomClientSecret,
                request.ZoomHostEmail, request.ZoomUserId,
                request.JitsiDomain, request.JitsiAppId, request.JitsiAppSecret,
                request.GoogleServiceAccountJson, request.GoogleHostEmail));

            var roster = await UpsertRoster(entity.Id, request.ShiftId, request.LeaveGroupId, request.JoiningDate, true);
            var position = await UpsertPosition(entity.Id, request.DepartmentId, request.DesignationId, request.GradeId, request.JoiningDate, request.JobResponsibility, true);
            var address = await UpsertAddress(entity.Id, request.AddressTypeId, request.Address, request.AddressCountryId, request.AddressCityId, true);
            var subCoordinate = await InsertSubCoordinateIfNeeded(entity.Id, request.SubCampusId, request.SubCampusFromDate, request.SubCampusTillDate, request.SubCampusRemarks);

            if (vacancyPosition != null)
            {
                await _vacancyFulfillmentService.FillFromNewEmployeeAsync(
                    vacancyPosition.Id, entity.Id, clientId, entityId.Value, GetActor(), CancellationToken.None);
            }

            await transaction.CommitAsync();

            // UpsertMeetingHostInfo hands back the Zoom row; re-read so the response
            // carries whatever Jitsi row was written alongside it.
            var meetingHostRows = await _context.MeetingHostInfos.AsNoTracking()
                .Where(x => x.EmployeeId == entity.Id && !x.UtBlock)
                .ToListAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id },
                MapToDto(entity, roster, address, subCoordinate, position,
                    PickHost(meetingHostRows, LovTypeZoom), PickHost(meetingHostRows, LovTypeJitsi),
                    PickHost(meetingHostRows, LovTypeGoogleMeet)));
        }

        /// <summary>
        /// Lightweight update used by LMS "Hero" modal to sync instructor selection back to HR profile.
        /// Avoids full EmployeeProfile validation (which requires many fields).
        /// </summary>
        [HttpPut("lms-instructor")]
        public async Task<IActionResult> UpdateLmsInstructor([FromBody] UpdateEmployeeLmsInstructorRequest request)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            _logger.LogInformation("UpdateLmsInstructor: EmployeeId={EmployeeId}, DesignationId={DesignationId}, ImageId={ImageId}, HeaderEntityId={HeaderEntityId}", 
                request.EmployeeId, request.DesignationId, request.ImageId, headerEntityId);

            var entity = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(x =>
                    x.Id == request.EmployeeId &&
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false));

            if (entity == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found or blocked for ClientId {ClientId}", request.EmployeeId, clientId);
                return NotFound();
            }

            if (headerEntityId.HasValue && entity.EntityId != headerEntityId.Value)
            {
                _logger.LogWarning("Employee {EmployeeId} EntityId mismatch. Expected {Expected}, Found {Found}. Loosening check...", 
                    request.EmployeeId, headerEntityId.Value, entity.EntityId);
                // We'll allow the update if it's the same client, as instructors might be shared across entities
            }

            if (request.DesignationId.HasValue)
            {
                _logger.LogInformation("Updating Employee {EmployeeId} DesignationId from {Old} to {New}", request.EmployeeId, entity.DesignationId, request.DesignationId.Value);
                entity.DesignationId = request.DesignationId.Value;
            }

            if (request.ImageId.HasValue)
            {
                _logger.LogInformation("Updating Employee {EmployeeId} ImageId from {Old} to {New}", request.EmployeeId, entity.ImageId, request.ImageId.Value);
                entity.ImageId = request.ImageId.Value;
            }

            entity.UtEditTime = DateTime.UtcNow;
            entity.UtRecordVersion++;

            try
            {
                int changes = await _context.SaveChangesAsync();
                _logger.LogInformation("UpdateLmsInstructor completed for Employee {EmployeeId}. Changes in DB: {Changes}", request.EmployeeId, changes);
                
                if (changes == 0)
                {
                    _logger.LogWarning("No changes were saved to the database for Employee {EmployeeId}. Check if data is truly different.", request.EmployeeId);
                }
                
                // Final verification log
                var updated = await _context.EmployeeProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.EmployeeId);
                if (updated != null)
                {
                    _logger.LogInformation("Post-save state for {EmployeeId}: DesignationId={Desig}, ImageId={Img}", 
                        request.EmployeeId, updated.DesignationId, updated.ImageId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save LMS instructor update for Employee {EmployeeId}", request.EmployeeId);
                return StatusCode(500, new { error = "Database update failed", detail = ex.Message });
            }

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeProfileV3Request request)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            var teachingSubjectTypes = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingSubjectTypeIds);
            var teachingClasses = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingClassIds);
            var teachingCategories = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingCourseCategoryIds);
            var teachingCourses = ResolveTeachingIds(request.EmployeeCategoryId, request.TeachingCourseIds);
            var courseCategoryId = ResolveCourseCategoryId(request.CourseCategoryId, teachingCategories);

            var errors = ValidateRequest(
                request.FirstName,
                request.PositionTypeId,
                request.JoiningDate,
                request.EmployeeTypeId,
                request.EmployeeCategoryId,
                courseCategoryId,
                request.DateOfBirth,
                request.DepartmentId,
                request.DesignationId,
                request.StatusId,
                request.GradeId,
                request.ResignDate,
                request.GenderId,
                request.BirthCountryId,
                request.BirthCityId,
                request.NationalityId,
                request.CellNo,
                request.ShiftId);

            if (errors.Count > 0)
            {
                return BadRequest(string.Join(" ", errors));
            }
            if (NeedsTeachingClassIdsColumn(teachingClasses, teachingCategories, teachingCourses) && !await TeachingClassIdsColumnExistsAsync())
            {
                return BadRequest(TeachingClassIdsColumnMissingMessage);
            }

            var entity = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id &&
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false) &&
                    (!headerEntityId.HasValue || x.EntityId == headerEntityId.Value));

            if (entity == null) return NotFound();

            var requestedEntityId = NormalizeId(request.EntityId) ?? entity.EntityId;
            if (!requestedEntityId.HasValue)
            {
                return BadRequest("EntityId is required.");
            }
            // A scheduled promotion retains the employee's current campus until it takes effect.
            var subCampusParentId = request.VacancyPositionId.HasValue
                ? entity.EntityId ?? requestedEntityId.Value
                : requestedEntityId.Value;
            if (!await IsSubCampusInScope(request.SubCampusId, clientId, subCampusParentId))
            {
                return BadRequest("Selected sub campus does not belong to the employee entity. Please select a sub campus of the selected campus or choose None.");
            }
            if (!await IsGradeInScope(request.GradeId, clientId))
            {
                return BadRequest("Grade is not valid for the current client.");
            }
            var reportingToError = await ValidateReportingToEmployeeAsync(
                request.ReportingToEmployeeId,
                request.Id,
                clientId,
                requestedEntityId.Value);
            if (reportingToError != null)
            {
                return BadRequest(reportingToError);
            }

            var substituteError = await ValidateSubstituteAsync(request.SubstituteEmployeeId, request.Id, clientId, requestedEntityId.Value);
            if (substituteError != null) return BadRequest(substituteError);

            if (!await _context.OrgLegalentities.AsNoTracking().AnyAsync(x =>
                    x.Id == requestedEntityId.Value &&
                    !x.UtBlock &&
                    (!x.ClientId.HasValue || x.ClientId == clientId)))
            {
                return BadRequest("Selected entity does not belong to the current client.");
            }

            HRVacancy? promotionVacancy = null;
            var isPromotionFulfillment = request.VacancyPositionId.HasValue;
            if (isPromotionFulfillment)
            {
                if (!request.PromotionEffectiveDate.HasValue || request.PromotionEffectiveDate.Value == default)
                    return BadRequest("Promotion effective date is required.");

                try
                {
                    var targetPosition = await _vacancyFulfillmentService.ValidateAvailablePositionAsync(
                        request.VacancyPositionId!.Value, clientId, requestedEntityId.Value, CancellationToken.None);
                    promotionVacancy = await _context.Vacancies.AsNoTracking()
                        .FirstAsync(x => x.Id == targetPosition.VacancyId);
                }
                catch (InvalidOperationException exception)
                {
                    return BadRequest(exception.Message);
                }

                if (request.DepartmentId != promotionVacancy.DepartmentId
                    || request.DesignationId != promotionVacancy.DesignationId
                    || request.GradeId != promotionVacancy.GradeId
                    || (promotionVacancy.EmploymentTypeId.HasValue && request.EmployeeTypeId != promotionVacancy.EmploymentTypeId))
                {
                    return BadRequest("Employee position fields must match the selected vacancy position. Reload the vacancy defaults and try again.");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.EmployeeCode))
            {
                var code = request.EmployeeCode.Trim();
                if (await _context.EmployeeProfiles.AnyAsync(x =>
                        x.EmployeeCode == code &&
                        x.ClientId == clientId &&
                        x.Id != request.Id &&
                        !(x.UtBlock ?? false)))
                {
                    return BadRequest($"Employee Code '{code}' already exists.");
                }
            }

            var isEmployeeExit = IsResignedStatus(request.StatusId) && request.ResignDate.HasValue;
            var isEmployeeExitDue = isEmployeeExit && request.ResignDate!.Value.Date <= DateTime.UtcNow.Date;

            // Keep employee changes together with either vacancy reservation or
            // position release. A failed occupancy update must not leave the profile
            // and organogram disagreeing about whether this employee is active.
            await using var profileTransaction = isPromotionFulfillment || isEmployeeExitDue
                ? await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable)
                : null;

            if (!isPromotionFulfillment && entity.EntityId != requestedEntityId.Value)
            {
                entity.PreviousEntityId = entity.EntityId;
                entity.EntityId = requestedEntityId.Value;
            }

            entity.EmployeeCode = request.EmployeeCode?.Trim();
            entity.MachineCode = request.MachineCode?.Trim();
            entity.FirstName = request.FirstName?.Trim();
            entity.LastName = request.LastName?.Trim();
            entity.ParentName = request.ParentName?.Trim();
            entity.PositionTypeId = request.PositionTypeId;
            entity.EmployeeTypeId = isPromotionFulfillment ? entity.EmployeeTypeId : request.EmployeeTypeId;
            entity.EmployeeCategoryId = request.EmployeeCategoryId;
            entity.CourseCategoryId = request.EmployeeCategoryId == TeacherCategoryId ? courseCategoryId : null;
            entity.DepartmentId = isPromotionFulfillment ? entity.DepartmentId : request.DepartmentId;
            entity.DesignationId = isPromotionFulfillment ? entity.DesignationId : request.DesignationId;
            entity.ReportingToEmployeeId = NormalizeId(request.ReportingToEmployeeId);
            entity.SubstituteEmployeeId = NormalizeId(request.SubstituteEmployeeId);
            entity.StatusId = request.StatusId;
            entity.GradeId = isPromotionFulfillment ? entity.GradeId : request.GradeId;
            entity.JoiningDate = request.JoiningDate;
            entity.ResignDate = request.ResignDate;
            entity.DateOfBirth = request.DateOfBirth;
            entity.GenderId = request.GenderId;
            entity.BloodGroupId = request.BloodGroupId;
            entity.MaritalStatusId = request.MaritalStatusId;
            entity.BirthCountryId = request.BirthCountryId;
            entity.BirthCityId = request.BirthCityId;
            entity.NationalityId = request.NationalityId;
            entity.ReligionId = request.ReligionId;
            entity.EmailId = request.EmailId?.Trim();
            entity.CellNo = request.CellNo?.Trim();
            entity.CnicNo = request.CnicNo?.Trim();
            entity.SessiNo = request.SessiNo?.Trim();
            entity.EobiNo = request.EobiNo?.Trim();
            // New EREmployeeinfohdr columns: null on update means "not sent", so existing values are kept.
            if (request.CprNo != null) entity.CprNo = request.CprNo.Trim();
            if (request.CprExpiry.HasValue) entity.CprExpiry = request.CprExpiry;
            if (request.PassportNo != null) entity.PassportNo = request.PassportNo.Trim();
            if (request.PassportExpiry.HasValue) entity.PassportExpiry = request.PassportExpiry;
            if (request.VisaNo != null) entity.VisaNo = request.VisaNo.Trim();
            if (request.VisaExpiry.HasValue) entity.VisaExpiry = request.VisaExpiry;
            if (request.MoeId.HasValue) entity.MoeId = request.MoeId;
            if (request.MoeExpiry.HasValue) entity.MoeExpiry = request.MoeExpiry;
            if (request.HeaderAddress != null) entity.HeaderAddress = request.HeaderAddress.Trim();
            if (request.DrivingLicenseNo != null) entity.DrivingLicenseNo = request.DrivingLicenseNo.Trim();
            if (request.LicenseExpiry.HasValue) entity.LicenseExpiry = request.LicenseExpiry;
            if (request.EmergencyNo != null) entity.EmergencyNo = request.EmergencyNo.Trim();
            if (request.VehicleId.HasValue) entity.VehicleId = request.VehicleId;
            if (request.HomeStopId.HasValue) entity.HomeStopId = request.HomeStopId;
            if (request.TValidFrom.HasValue) entity.TValidFrom = request.TValidFrom;
            if (request.TValidTill.HasValue) entity.TValidTill = request.TValidTill;
            if (request.TActive.HasValue) entity.TActive = request.TActive.Value;
            entity.ShiftId = request.ShiftId;
            entity.ImageId = request.ImageId;

            // Tracked here so the subject skills save together with the employee below.
            if (teachingSubjectTypes.Write)
                await SyncTeachingSubjectTypesAsync(entity.Id, clientId, teachingSubjectTypes.Value);

            await _context.SaveChangesAsync();

            await WriteTeachingSpecializationAsync(entity.Id, clientId, teachingClasses, teachingCategories, teachingCourses);

            await UpsertMeetingHostInfo(entity.Id, entity.ClientId, new MeetingHostCredentials(
                request.ZoomUsername, request.ZoomPassword, request.ZoomMeetingId, request.ZoomMeetingPassword,
                request.ZoomInvitePassword, request.ZoomApiKey, request.ZoomApiSecret, request.ZoomSdkKey,
                request.ZoomSdkSecret, request.ZoomToken, request.ZoomAccountId, request.ZoomClientSecret,
                request.ZoomHostEmail, request.ZoomUserId,
                request.JitsiDomain, request.JitsiAppId, request.JitsiAppSecret,
                request.GoogleServiceAccountJson, request.GoogleHostEmail));

            await UpsertRoster(entity.Id, request.ShiftId, request.LeaveGroupId, request.JoiningDate, false);
            if (!isPromotionFulfillment && !isEmployeeExitDue)
                await UpsertPosition(entity.Id, request.DepartmentId, request.DesignationId, request.GradeId, request.JoiningDate, request.JobResponsibility, false);
            await UpsertAddress(entity.Id, request.AddressTypeId, request.Address, request.AddressCountryId, request.AddressCityId, false);
            await InsertSubCoordinateIfNeeded(entity.Id, request.SubCampusId, request.SubCampusFromDate, request.SubCampusTillDate, request.SubCampusRemarks);

            if (isEmployeeExitDue)
            {
                try
                {
                    await _vacancyFulfillmentService.ReleaseEmployeePositionsAsync(
                        entity.Id,
                        clientId,
                        requestedEntityId.Value,
                        request.ResignDate!.Value,
                        GetActor(),
                        "Employee resigned",
                        CancellationToken.None);
                }
                catch (InvalidOperationException exception)
                {
                    if (profileTransaction != null)
                        await profileTransaction.RollbackAsync();
                    return BadRequest(exception.Message);
                }
            }

            if (isPromotionFulfillment)
            {
                await _vacancyFulfillmentService.SchedulePromotionAsync(
                    request.VacancyPositionId!.Value,
                    entity.Id,
                    request.PromotionEffectiveDate!.Value,
                    request.PromotionRemarks,
                    clientId,
                    requestedEntityId.Value,
                    GetActor(),
                    CancellationToken.None);
            }

            if (profileTransaction != null)
                await profileTransaction.CommitAsync();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            var entity = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false) &&
                    (!headerEntityId.HasValue || x.EntityId == headerEntityId.Value));

            if (entity == null) return NotFound();

            if (await HasAttendanceForEmployee(entity.Id, clientId, headerEntityId))
            {
                return BadRequest("Attendance exists against this employee. Record cannot be deleted.");
            }

            entity.UtBlock = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("lookups/shifts")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetShiftLookup(
            [FromQuery] int? entityId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? employeeId = null)
        {
            var clientId = GetClientId();
            var effectiveEntityId = NormalizeId(entityId) ?? GetEntityId();
            var effectiveDepartmentId = NormalizeId(departmentId);
            var effectiveEmployeeId = NormalizeId(employeeId);
            var departmentSharedWithIds = await _context.Lovs.AsNoTracking()
                .Where(lov => lov.Vtype == "SCHFR" && !lov.UtBlock &&
                    ((lov.Stxt != null && lov.Stxt.ToLower().Contains("department")) ||
                     (lov.Ltxt != null && lov.Ltxt.ToLower().Contains("department")) ||
                     (lov.Code != null && lov.Code.ToLower().Contains("dept"))))
                .Select(lov => lov.Id)
                .ToListAsync();

            var query = _context.Shifts.AsNoTracking()
                .Where(x => x.ClientId == clientId);

            if (effectiveEntityId.HasValue || effectiveDepartmentId.HasValue || effectiveEmployeeId.HasValue)
            {
                query = query.Where(x =>
                    _context.ShiftSharedDetails.Any(shared =>
                        shared.ShiftId == x.Id &&
                        shared.PClientId == clientId &&
                        effectiveDepartmentId.HasValue &&
                        shared.PDepartId == effectiveDepartmentId.Value) ||
                    _context.ShiftSharedDetails.Any(shared =>
                        shared.ShiftId == x.Id &&
                        shared.PClientId == clientId &&
                        effectiveEmployeeId.HasValue &&
                        shared.PEmpId == effectiveEmployeeId.Value) ||
                    (!_context.ShiftSharedDetails.Any(shared =>
                        shared.ShiftId == x.Id &&
                        shared.PClientId == clientId &&
                        shared.PDepartId.HasValue) &&
                     !departmentSharedWithIds.Contains(x.LovSharedId ?? 0) &&
                     _context.ShiftSharedDetails.Any(shared =>
                            shared.ShiftId == x.Id &&
                            shared.PClientId == clientId &&
                            effectiveEntityId.HasValue &&
                            shared.PEntityId == effectiveEntityId.Value)) ||
                    (!departmentSharedWithIds.Contains(x.LovSharedId ?? 0) &&
                     !_context.ShiftSharedDetails.Any(shared =>
                        shared.ShiftId == x.Id &&
                        shared.PClientId == clientId) &&
                     effectiveEntityId.HasValue && x.EntityId == effectiveEntityId.Value));
            }

            var items = await query
                .OrderBy(x => x.Stxt)
                .Select(x => new LookupItemDto(x.Id, x.Code, x.Stxt))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/leave-groups")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetLeaveGroupLookup()
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            var query = _context.LeaveGroups.AsNoTracking()
                .Where(x => x.ClientId == clientId && !(x.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.EntityId == entityId.Value);
            }

            var items = await query
                .OrderBy(x => x.Stxt)
                .Select(x => new LookupItemDto(x.Id, x.Code, x.Stxt))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/reporting-to")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetReportingToLookup(
            [FromQuery] int? entityId = null,
            [FromQuery] int? excludeEmployeeId = null)
        {
            var clientId = GetClientId();
            var effectiveEntityId = NormalizeId(entityId) ?? GetEntityId();
            if (!effectiveEntityId.HasValue)
            {
                return Ok(Array.Empty<LookupItemDto>());
            }

            var query = await ReportingManagerCandidatesAsync(clientId, effectiveEntityId.Value);

            if (excludeEmployeeId.HasValue && excludeEmployeeId.Value > 0)
            {
                query = query.Where(x => x.Id != excludeEmployeeId.Value);
            }

            var items = await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.EmployeeCode)
                .Select(x => new LookupItemDto(
                    x.Id,
                    x.EmployeeCode,
                    ((x.FirstName ?? string.Empty) + " " + (x.LastName ?? string.Empty)).Trim()))
                .ToListAsync();

            return Ok(items);
        }

        private static bool IsReportingManagerDesignation(string? value) =>
            !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value,
                @"\b(?:managers?|mgr|c\.?e\.?o\.?)\b|\bchief\s+executive(?:\s+officer)?\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private async Task<IQueryable<EmployeeProfileLegacy>> ReportingManagerCandidatesAsync(int clientId, int entityId)
        {
            // Include leadership before their first direct report is assigned.
            // Match designation words, not employee names or "Management Trainee".
            var designations = await _context.Designations.AsNoTracking()
                .Where(x => x.ClientId == clientId && !(x.UtBlock ?? false))
                .Select(x => new { x.Id, x.Code, x.Stxt }).ToListAsync();
            var leadershipDesignationIds = designations
                .Where(x => IsReportingManagerDesignation(x.Stxt) || IsReportingManagerDesignation(x.Code))
                .Select(x => x.Id).ToArray();
            return _context.EmployeeProfiles.AsNoTracking().Where(x =>
                x.ClientId == clientId && x.EntityId == entityId && !(x.UtBlock ?? false) &&
                ((x.DesignationId.HasValue && leadershipDesignationIds.Contains(x.DesignationId.Value)) ||
                 _context.EmployeeProfiles.Any(report => report.ClientId == clientId && report.EntityId == entityId &&
                     !(report.UtBlock ?? false) && report.Id != x.Id && report.ReportingToEmployeeId == x.Id)));
        }

        private async Task<string?> ValidateSubstituteAsync(int? substituteId, int employeeId, int clientId, int entityId)
        {
            var selectedId = NormalizeId(substituteId);
            if (!selectedId.HasValue) return null;
            if (selectedId.Value == employeeId) return "An employee cannot be their own Substitute.";
            var candidates = await ReportingManagerCandidatesAsync(clientId, entityId);
            return await candidates.AnyAsync(x => x.Id == selectedId.Value) ? null
                : "Substitute must be a line manager in the selected entity.";
        }

        [HttpGet("lookups/grades")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetGradeLookup()
        {
            var clientId = GetClientId();
            var items = await _context.EmployeeGrades.AsNoTracking()
                .Where(x => x.ClientId == clientId && !x.UtBlock)
                .OrderBy(x => x.Code)
                .Select(x => new LookupItemDto(x.Id, x.Code, x.Code))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/sub-campuses")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetSubCampusLookup([FromQuery] int? entityId = null)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();
            var effectiveEntityId = entityId ?? headerEntityId;
            if (!effectiveEntityId.HasValue)
            {
                return Ok(Array.Empty<LookupItemDto>());
            }

            var items = await SubCampusesInScope(clientId, effectiveEntityId.Value)
                .OrderBy(x => x.Stxt ?? x.CampusName ?? x.ShortName ?? x.EntityNo)
                .Select(x => new LookupItemDto(
                    x.Id,
                    x.EntityNo,
                    x.CampusName ?? x.Stxt ?? x.ShortName ?? x.EntityNo))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}/sub-coordinates")]
        public async Task<ActionResult<IEnumerable<EmployeeSubCoordinateV3Dto>>> GetSubCoordinates(int id)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            var employeeExists = await _context.EmployeeProfiles.AsNoTracking()
                .AnyAsync(x =>
                    x.Id == id &&
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false) &&
                    (!headerEntityId.HasValue || x.EntityId == headerEntityId.Value));

            if (!employeeExists) return NotFound();

            var rows = await (
                from subCoordinate in _context.EmployeeSubCoordinates.AsNoTracking()
                join campus in _context.OrgLegalentities.AsNoTracking()
                    on subCoordinate.SubCampusId equals campus.Id into campusJoin
                from campus in campusJoin.DefaultIfEmpty()
                where subCoordinate.EmployeeId == id && !(subCoordinate.UtBlock ?? false)
                orderby subCoordinate.FromDate descending, subCoordinate.Id descending
                select new EmployeeSubCoordinateV3Dto(
                    subCoordinate.Id,
                    subCoordinate.EmployeeId ?? 0,
                    subCoordinate.SubCampusId,
                    campus.EntityNo,
                    campus.CampusName ?? campus.Stxt ?? campus.ShortName ?? campus.EntityNo,
                    subCoordinate.FromDate,
                    subCoordinate.TillDate,
                    subCoordinate.Remarks))
                .ToListAsync();

            return Ok(rows);
        }

        [HttpDelete("sub-coordinates/{id:int}")]
        public async Task<IActionResult> DeleteSubCoordinate(int id)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            var subCoordinate = await _context.EmployeeSubCoordinates
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !(x.UtBlock ?? false) &&
                    x.Employee != null &&
                    x.Employee.ClientId == clientId &&
                    !(x.Employee.UtBlock ?? false) &&
                    (!headerEntityId.HasValue || x.Employee.EntityId == headerEntityId.Value));

            if (subCoordinate == null) return NotFound();

            subCoordinate.UtBlock = true;
            subCoordinate.UtEditTime = DateTime.UtcNow;
            subCoordinate.UtEditUser = GetUserName();
            subCoordinate.UtRecordVersion = subCoordinate.UtRecordVersion <= 0 ? 1 : subCoordinate.UtRecordVersion + 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<EmployeeProfileDefaultsV3Dto> ResolveEmployeeProfileDefaults(int clientId, int? entityId)
        {
            int? birthCountryId = null;
            int? birthCityId = null;

            if (entityId.HasValue)
            {
                var policy = await _context.SystemPolicies.AsNoTracking()
                    .Where(x =>
                        x.EntityId == entityId.Value &&
                        !(x.UtBlock ?? false) &&
                        (!x.ClientId.HasValue || x.ClientId == clientId))
                    .OrderByDescending(x => x.ClientId == clientId)
                    .ThenByDescending(x => x.Id)
                    .Select(x => new
                    {
                        x.DefaultCountryId,
                        x.DefaultCityId
                    })
                    .FirstOrDefaultAsync();

                birthCountryId = NormalizeId(policy?.DefaultCountryId);
                birthCityId = NormalizeId(policy?.DefaultCityId);

                if (!birthCountryId.HasValue || !birthCityId.HasValue)
                {
                    var legalEntity = await _context.OrgLegalentities.AsNoTracking()
                        .Where(x =>
                            x.Id == entityId.Value &&
                            !x.UtBlock &&
                            (!x.ClientId.HasValue || x.ClientId == clientId))
                        .Select(x => new
                        {
                            x.CountryId,
                            x.CityId
                        })
                        .FirstOrDefaultAsync();

                    birthCountryId ??= NormalizeId(legalEntity?.CountryId);
                    birthCityId ??= NormalizeId(legalEntity?.CityId);
                }
            }

            return new EmployeeProfileDefaultsV3Dto(
                birthCountryId,
                birthCityId,
                birthCountryId);
        }

        private static int? NormalizeId(int? value)
        {
            return value.HasValue && value.Value > 0 ? value.Value : null;
        }

        private sealed record TeachingIdsChange(bool Write, int[] Value);

        // Teaching Specialization subject types live in EREmployeesubjectskill, which the
        // Subject Rating tab also owns, so only a teacher's save touches them: changing the
        // category to a non-teacher must not erase ratings. Classes, Teaching Levels and
        // courses follow the same rule.
        // Null means the client did not load the field (leave it alone) and an empty list
        // means every entry was removed.
        private static TeachingIdsChange ResolveTeachingIds(int? employeeCategoryId, int[]? ids)
        {
            if (employeeCategoryId != TeacherCategoryId || ids == null)
                return new TeachingIdsChange(false, Array.Empty<int>());
            return new TeachingIdsChange(true, ids.Where(id => id > 0).Distinct().ToArray());
        }

        // Teaching Level is multi-select: the first one stays in coursecategoryid, which
        // the rest of the system reads, and the full list goes to teachingclassids. Null
        // categories (not loaded) keep the CourseCategoryId the client sent.
        private static int? ResolveCourseCategoryId(int? courseCategoryId, TeachingIdsChange categories)
        {
            if (!categories.Write) return courseCategoryId;
            return categories.Value.Length > 0 ? categories.Value[0] : null;
        }

        // A single category fits in coursecategoryid, so only classes, courses or a second
        // category need the teachingclassids column.
        private static bool NeedsTeachingClassIdsColumn(TeachingIdsChange classes, TeachingIdsChange categories, TeachingIdsChange courses)
        {
            return classes.Value.Length > 0 || courses.Value.Length > 0 || categories.Value.Length > 1;
        }

        private const string TeachingClassIdsColumnMissingMessage =
            "Teaching Specialization cannot be saved: column teachingclassids has not been added to EREmployeeinfohdr in this database.";

        private sealed record TeachingSpecialization(int[] ClassIds, int[] CourseCategoryIds, int[] CourseIds);

        // The object form of teachingclassids, used once Teaching Levels or courses are chosen.
        private sealed record TeachingSpecializationJson(int[]? Programs, int[]? Categories, int[]? Courses);

        private static readonly JsonSerializerOptions TeachingSpecializationJsonOptions = new(JsonSerializerDefaults.Web);

        // EREmployeeinfohdr.teachingclassids holds the classes (SMCourse ids) a teacher can
        // teach as a JSON array such as [20480,20481], or, when Teaching Levels (COTY lov ids)
        // or courses (SMSubject ids) are chosen too,
        // {"programs":[20480,20481],"categories":[1101],"courses":[45404]}. It is
        // not mapped on EmployeeProfileLegacy, because every employee query would fail on a
        // database that has not added the column yet, so it is read and written with SQL
        // that checks for it.
        private async Task<bool> TeachingClassIdsColumnExistsAsync()
        {
            var values = await _context.Database.SqlQueryRaw<int>(@"
SELECT CASE WHEN COL_LENGTH(N'dbo.EREmployeeinfohdr', N'teachingclassids') IS NULL THEN 0 ELSE 1 END AS Value")
                .ToListAsync();
            return values.FirstOrDefault() == 1;
        }

        private async Task<TeachingSpecialization> ReadTeachingSpecializationAsync(int employeeId, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            var scopeEntityId = entityId ?? 0;
            var values = await _context.Database.SqlQuery<string>($@"
IF COL_LENGTH(N'dbo.EREmployeeinfohdr', N'teachingclassids') IS NOT NULL
    EXEC sp_executesql
        N'SELECT ISNULL(teachingclassids, N'''') AS Value FROM dbo.EREmployeeinfohdr
          WHERE Id = @employeeId AND clientid = @clientId AND (@entityId = 0 OR entityid = @entityId)',
        N'@employeeId int, @clientId int, @entityId int',
        @employeeId = {employeeId}, @clientId = {clientId}, @entityId = {scopeEntityId};
ELSE
    SELECT CAST(N'' AS nvarchar(max)) AS Value WHERE 1 = 0;")
                .ToListAsync(cancellationToken);

            var json = values.FirstOrDefault();
            var empty = new TeachingSpecialization(Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
            if (string.IsNullOrWhiteSpace(json)) return empty;
            try
            {
                static int[] Clean(int[]? ids) => (ids ?? Array.Empty<int>()).Where(id => id > 0).Distinct().ToArray();
                if (json.TrimStart().StartsWith('['))
                    return empty with { ClassIds = Clean(JsonSerializer.Deserialize<int[]>(json)) };
                var stored = JsonSerializer.Deserialize<TeachingSpecializationJson>(json, TeachingSpecializationJsonOptions);
                return new TeachingSpecialization(Clean(stored?.Programs), Clean(stored?.Categories), Clean(stored?.Courses));
            }
            catch (JsonException)
            {
                _logger.LogWarning("Employee {EmployeeId} has unreadable teachingclassids {Value}", employeeId, json);
                return empty;
            }
        }

        // Writes straight to the row, so it runs after the employee is saved and inside the
        // caller's transaction when there is one. Classes, categories and courses share the
        // column, so a list the client did not send is read back and kept. Classes and
        // courses outside the client are dropped; with no categories or courses the plain
        // class array is stored, and an empty selection stores NULL. Without the column
        // there is nothing to write.
        private async Task WriteTeachingSpecializationAsync(int employeeId, int clientId, TeachingIdsChange classes, TeachingIdsChange categories, TeachingIdsChange courses)
        {
            if (!classes.Write && !categories.Write && !courses.Write) return;

            var stored = classes.Write && categories.Write && courses.Write
                ? null
                : await ReadTeachingSpecializationAsync(employeeId, clientId, null, CancellationToken.None);
            var classIds = classes.Write ? classes.Value : stored!.ClassIds;
            var categoryIds = categories.Write ? categories.Value : stored!.CourseCategoryIds;
            var courseIds = courses.Write ? courses.Value : stored!.CourseIds;

            var validClassIds = classIds.Length == 0
                ? new List<int>()
                : await _context.Classes
                    .AsNoTracking()
                    .Where(x => classIds.Contains(x.Id) && x.ClientId == clientId && !x.UtBlock)
                    .Select(x => x.Id)
                    .ToListAsync();
            var programIds = classIds.Where(validClassIds.Contains).ToArray();
            // SMSubject is not mapped here; the ids are ints, so listing them inline is safe.
            var validCourseIds = courseIds.Length == 0
                ? new List<int>()
                : await _context.Database.SqlQueryRaw<int>(
                        $"SELECT Id AS Value FROM dbo.SMSubject WHERE clientid = @clientId AND ISNULL(utblock, 0) = 0 AND Id IN ({string.Join(",", courseIds)})",
                        new SqlParameter("@clientId", clientId))
                    .ToListAsync();
            var subjectIds = courseIds.Where(validCourseIds.Contains).ToArray();
            var json = categoryIds.Length > 0 || subjectIds.Length > 0
                ? JsonSerializer.Serialize(new TeachingSpecializationJson(programIds, categoryIds, subjectIds), TeachingSpecializationJsonOptions)
                : programIds.Length > 0
                    ? JsonSerializer.Serialize(programIds)
                    : null;

            await _context.Database.ExecuteSqlAsync($@"
IF COL_LENGTH(N'dbo.EREmployeeinfohdr', N'teachingclassids') IS NOT NULL
    EXEC sp_executesql
        N'UPDATE dbo.EREmployeeinfohdr SET teachingclassids = @classIds WHERE Id = @employeeId',
        N'@classIds nvarchar(max), @employeeId int',
        @classIds = {json}, @employeeId = {employeeId};");
        }

        // Stages, without saving, the employee's EREmployeesubjectskill rows to match
        // subjectTypeIds: unselected types are blocked the way the Subject Rating tab deletes
        // them, and new types get an unrated row. Rows of types still selected keep their
        // rating. Types outside the client are ignored.
        private async Task SyncTeachingSubjectTypesAsync(int employeeId, int clientId, int[] subjectTypeIds)
        {
            var validTypeIds = subjectTypeIds.Length == 0
                ? new List<int>()
                : await _context.SubjectTypes
                    .AsNoTracking()
                    .Where(x => subjectTypeIds.Contains(x.Id) && x.ClientId == clientId && !(x.UtBlock ?? false))
                    .Select(x => x.Id)
                    .ToListAsync();

            var rows = await _context.EmployeeSubjectRatings
                .Where(x => x.EmployeeId == employeeId && !x.UtBlock && x.SubjectTypeId != null)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var userName = GetUserName();
            foreach (var row in rows.Where(x => !validTypeIds.Contains(x.SubjectTypeId!.Value)))
            {
                row.UtBlock = true;
                row.UtEditTime = now;
                row.UtEditUser = userName;
                row.UtRecordVersion += 1;
            }

            var newTypeIds = validTypeIds
                .Where(subjectTypeId => rows.All(x => x.SubjectTypeId != subjectTypeId))
                .ToList();
            if (newTypeIds.Count == 0) return;

            var nextId = (await _context.EmployeeSubjectRatings.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
            foreach (var subjectTypeId in newTypeIds)
            {
                _context.EmployeeSubjectRatings.Add(new EmployeeSubjectRatingLegacy
                {
                    Id = nextId++,
                    EmployeeId = employeeId,
                    SubjectTypeId = subjectTypeId,
                    UtSysGen = false,
                    UtBlock = false,
                    UtIsPartial = false,
                    UtAddTime = now,
                    UtAddUser = userName,
                    UtRecordVersion = 1
                });
            }
        }

        private static EmployeeProfileV3Dto MapToDto(
            EmployeeProfileLegacy employee,
            EmployeeRosterLegacy? roster,
            EmployeeAddressLegacy? address,
            EmployeeSubCoordinateLegacy? subCoordinate,
            EREmpPositionLegacy? position,
            MeetingHostInfoLegacy? host,
            MeetingHostInfoLegacy? jitsiHost,
            MeetingHostInfoLegacy? googleHost)
        {
            return new EmployeeProfileV3Dto(
                employee.Id,
                employee.EntityId,
                employee.EmployeeCode,
                employee.MachineCode,
                employee.FirstName,
                employee.LastName,
                employee.ParentName,
                employee.PositionTypeId,
                employee.EmployeeTypeId,
                employee.EmployeeCategoryId,
                employee.CourseCategoryId,
                employee.DepartmentId,
                employee.DesignationId,
                employee.ReportingToEmployeeId,
                employee.StatusId,
                employee.JoiningDate,
                employee.ResignDate,
                employee.DateOfBirth,
                employee.GenderId,
                employee.BloodGroupId,
                employee.MaritalStatusId,
                employee.BirthCountryId,
                employee.BirthCityId,
                employee.NationalityId,
                employee.ReligionId,
                employee.EmailId,
                employee.CellNo,
                employee.CnicNo,
                employee.SessiNo,
                employee.EobiNo,
                roster?.ShiftId ?? employee.ShiftId,
                roster?.LeaveGroupId,
                address?.AddressTypeId,
                address?.Address,
                address?.CountryId,
                address?.CityId,
                employee.ImageId,
                host?.ZoomHostEmail,
                host?.ZoomPassword,
                host?.ZoomMeetingId,
                host?.ZoomMeetingPassword,
                host?.ZoomInvitePassword,
                // ZoomApiKey/ZoomApiSecret are the retired JWT slots, now the OAuth pair.
                null,
                host?.ZoomSdkKey,
                null,
                host?.ZoomSdkSecret,
                host?.ZoomAccountId,
                host?.ZoomClientSecret,
                host?.ZoomHostEmail,
                null,
                // Jitsi's row reuses the same columns: username is the server domain,
                // and the retired JWT pair holds the app id and secret.
                jitsiHost?.ZoomHostEmail,
                jitsiHost?.ZoomAccountId,
                jitsiHost?.ZoomClientSecret,
                // Google's row keeps either a service-account key or an OAuth refresh
                // token in jwttoken. Only the key belongs to the form; a refresh token
                // is a live credential the browser has no use for, and returning it
                // would also make the tab think this host uses a service account.
                IsGoogleServiceAccountKey(googleHost?.ZoomToken) ? googleHost!.ZoomToken : null,
                googleHost?.ZoomHostEmail,
                subCoordinate?.SubCampusId,
                subCoordinate?.FromDate,
                subCoordinate?.TillDate,
                subCoordinate?.Remarks,
                host?.ZoomToken,
                position?.GradeId,
                position?.Responsibility,
                employee.CprNo,
                employee.CprExpiry,
                employee.PassportNo,
                employee.PassportExpiry,
                employee.VisaNo,
                employee.VisaExpiry,
                employee.MoeId,
                employee.MoeExpiry,
                employee.HeaderAddress,
                employee.DrivingLicenseNo,
                employee.LicenseExpiry,
                employee.EmergencyNo,
                employee.VehicleId,
                employee.HomeStopId,
                employee.TValidFrom,
                employee.TValidTill,
                employee.TActive,
                employee.SubstituteEmployeeId
            );
        }

        /// <summary>
        /// Picks one employee's row for a provider. Rows written before Jitsi existed
        /// carry no lovtypeid and belong to Zoom, so Zoom accepts an untagged row while
        /// Jitsi only ever matches its own tag - reading a Zoom row as Jitsi would treat
        /// the Zoom account id as a Jitsi app id.
        /// </summary>
        private static MeetingHostInfoLegacy? PickHost(IEnumerable<MeetingHostInfoLegacy> hosts, int lovTypeId)
        {
            var rows = hosts.Where(x => lovTypeId == LovTypeZoom
                ? x.LovTypeId == LovTypeZoom || x.LovTypeId == null
                : x.LovTypeId == lovTypeId);

            // A row explicitly tagged for this provider beats an untagged legacy one.
            return rows
                .OrderByDescending(x => x.LovTypeId != null)
                .ThenByDescending(x => x.Id)
                .FirstOrDefault();
        }

        /// <summary>
        /// A service-account key is JSON; an OAuth refresh token is an opaque string.
        /// The stored value identifies itself, so no extra column is needed to say
        /// which kind of Google credential a host has.
        /// </summary>
        private static bool IsGoogleServiceAccountKey(string? credential) =>
            !string.IsNullOrWhiteSpace(credential) && credential.TrimStart().StartsWith('{');

        private static EmployeeRosterLegacy? PickRoster(IEnumerable<EmployeeRosterLegacy> rosters, int employeeId)
        {
            return rosters
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.EndDate ?? MaxDate)
                .FirstOrDefault();
        }

        private static EmployeeAddressLegacy? PickAddress(IEnumerable<EmployeeAddressLegacy> addresses, int employeeId)
        {
            return addresses
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
        }

        private static EREmpPositionLegacy? PickPosition(IEnumerable<EREmpPositionLegacy> positions, int employeeId)
        {
            return positions.Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.TillDate ?? MaxDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefault();
        }

        private static EmployeeSubCoordinateLegacy? PickSubCoordinate(IEnumerable<EmployeeSubCoordinateLegacy> subCoordinates, int employeeId)
        {
            return subCoordinates
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.FromDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefault();
        }

        private async Task<List<EmployeeSubCoordinateLegacy>> LoadSubCoordinatesForEmployeesAsync(IReadOnlyCollection<int> employeeIds)
        {
            if (employeeIds.Count == 0) return new List<EmployeeSubCoordinateLegacy>();

            try
            {
                return await _context.EmployeeSubCoordinates.AsNoTracking()
                    .Where(x => x.EmployeeId != null && employeeIds.Contains(x.EmployeeId.Value) && !(x.UtBlock ?? false))
                    .ToListAsync();
            }
            catch (SqlException ex) when (IsInvalidObjectName(ex))
            {
                _logger.LogWarning(ex, "Employee sub-coordinate table is not available. Continuing without sub-coordinate data.");
                return new List<EmployeeSubCoordinateLegacy>();
            }
        }

        private async Task<EmployeeSubCoordinateLegacy?> LoadLatestSubCoordinateAsync(int employeeId)
        {
            try
            {
                return await _context.EmployeeSubCoordinates.AsNoTracking()
                    .Where(x => x.EmployeeId == employeeId && !(x.UtBlock ?? false))
                    .OrderByDescending(x => x.FromDate)
                    .ThenByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
            }
            catch (SqlException ex) when (IsInvalidObjectName(ex))
            {
                _logger.LogWarning(ex, "Employee sub-coordinate table is not available. Continuing without sub-coordinate data.");
                return null;
            }
        }

        private static bool IsInvalidObjectName(SqlException exception) =>
            exception.Errors.Cast<SqlError>().Any(error => error.Number == 208);

        /// <summary>
        /// The meeting credentials carried by both the create and update requests.
        /// </summary>
        private sealed record MeetingHostCredentials(
            string? ZoomUsername,
            string? ZoomPassword,
            string? ZoomMeetingId,
            string? ZoomMeetingPassword,
            string? ZoomInvitePassword,
            string? ZoomApiKey,
            string? ZoomApiSecret,
            string? ZoomSdkKey,
            string? ZoomSdkSecret,
            string? ZoomToken,
            string? ZoomAccountId,
            string? ZoomClientSecret,
            string? ZoomHostEmail,
            string? ZoomUserId,
            string? JitsiDomain,
            string? JitsiAppId,
            string? JitsiAppSecret,
            string? GoogleServiceAccountJson,
            string? GoogleHostEmail);

        /// <summary>
        /// Writes the online-meeting credentials to SMmeetinghostinfo, the single place
        /// every meeting integration reads host details from. One row per provider per
        /// employee, told apart by lovtypeid, because Zoom and Jitsi share the same
        /// three credential columns. Id is not an identity column on this legacy table,
        /// so the key is allocated the same way the other legacy tables here do it.
        /// </summary>
        private async Task<MeetingHostInfoLegacy?> UpsertMeetingHostInfo(
            int employeeId,
            int? clientId,
            MeetingHostCredentials request)
        {
            static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

            var rows = await _context.MeetingHostInfos
                .Where(x => x.EmployeeId == employeeId && !x.UtBlock)
                .ToListAsync();

            // Rows written before Jitsi existed carry no lovtypeid and are Zoom's, so an
            // untagged row is adopted as the Zoom row and stamped on the way out.
            var zoomRow = rows.FirstOrDefault(x => x.LovTypeId == LovTypeZoom)
                ?? rows.FirstOrDefault(x => x.LovTypeId == null);
            var jitsiRow = rows.FirstOrDefault(x => x.LovTypeId == LovTypeJitsi);
            var googleRow = rows.FirstOrDefault(x => x.LovTypeId == LovTypeGoogleMeet);

            var password = Clean(request.ZoomPassword);
            var meetingId = Clean(request.ZoomMeetingId);
            var meetingPwd = Clean(request.ZoomMeetingPassword);
            var invitePwd = Clean(request.ZoomInvitePassword);
            var sdkKey = Clean(request.ZoomSdkKey);
            var sdkSecret = Clean(request.ZoomSdkSecret);
            var token = Clean(request.ZoomToken);
            // These three reuse the retired JWT columns (apiKey/apiSecret/username).
            var accountId = Clean(request.ZoomAccountId);
            var clientSecret = Clean(request.ZoomClientSecret);
            var hostEmail = Clean(request.ZoomHostEmail) ?? Clean(request.ZoomUsername);
            // Jitsi reuses those same three columns on its own row: the domain stands in
            // for the host address, and the app id and secret for the OAuth pair.
            var jitsiDomain = Clean(request.JitsiDomain);
            var jitsiAppId = Clean(request.JitsiAppId);
            var jitsiAppSecret = Clean(request.JitsiAppSecret);
            // Google keeps the service-account key in jwttoken (nvarchar max) and the
            // impersonated address in username, on its own row.
            var googleJson = Clean(request.GoogleServiceAccountJson);
            var googleHostEmail = Clean(request.GoogleHostEmail);

            var hasZoomValue = password != null || meetingId != null
                || meetingPwd != null || invitePwd != null
                || sdkKey != null || sdkSecret != null || token != null || accountId != null
                || clientSecret != null || hostEmail != null;
            var hasJitsiValue = jitsiDomain != null || jitsiAppId != null || jitsiAppSecret != null;
            var hasGoogleValue = googleJson != null || googleHostEmail != null;

            // Nothing entered and nothing stored: no reason to create an empty host row.
            if (zoomRow == null && jitsiRow == null && googleRow == null
                && !hasZoomValue && !hasJitsiValue && !hasGoogleValue)
                return null;

            var nextId = (await _context.MeetingHostInfos.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;

            MeetingHostInfoLegacy Ensure(MeetingHostInfoLegacy? existing, int lovTypeId)
            {
                if (existing != null)
                {
                    // Stamp the provider onto a legacy row that predates lovtypeid.
                    existing.LovTypeId = lovTypeId;
                    return existing;
                }

                var created = new MeetingHostInfoLegacy
                {
                    Id = nextId++,
                    EmployeeId = employeeId,
                    LovTypeId = lovTypeId,
                    // All four are NOT NULL with no database default.
                    UtSysGen = false,
                    UtBlock = false,
                    UtIsPartial = false,
                    UtRecordVersion = 1
                };
                _context.MeetingHostInfos.Add(created);
                return created;
            }

            if (zoomRow != null || hasZoomValue)
            {
                var row = Ensure(zoomRow, LovTypeZoom);
                row.ClientId = clientId ?? row.ClientId;
                row.ZoomPassword = password;
                row.ZoomMeetingId = meetingId;
                row.ZoomMeetingPassword = meetingPwd;
                row.ZoomInvitePassword = invitePwd;
                row.ZoomSdkKey = sdkKey;
                row.ZoomSdkSecret = sdkSecret;
                row.ZoomToken = token;
                row.ZoomAccountId = accountId;
                row.ZoomClientSecret = clientSecret;
                row.ZoomHostEmail = hostEmail;
                zoomRow = row;
            }

            if (jitsiRow != null || hasJitsiValue)
            {
                var row = Ensure(jitsiRow, LovTypeJitsi);
                row.ClientId = clientId ?? row.ClientId;
                row.ZoomHostEmail = jitsiDomain;
                row.ZoomAccountId = jitsiAppId;
                row.ZoomClientSecret = jitsiAppSecret;
                jitsiRow = row;
            }

            if (googleRow != null || hasGoogleValue)
            {
                var row = Ensure(googleRow, LovTypeGoogleMeet);
                row.ClientId = clientId ?? row.ClientId;

                // A refresh token is written by the consent flow, not by this form, and
                // the form posts an empty key box whenever OAuth is the chosen method.
                // Only overwrite the stored credential when this save actually carries a
                // service-account key, or an ordinary profile edit would disconnect the
                // employee's Google account.
                var storedIsRefreshToken =
                    !string.IsNullOrWhiteSpace(row.ZoomToken) && !row.ZoomToken!.TrimStart().StartsWith('{');

                if (googleJson != null)
                {
                    row.ZoomToken = googleJson;
                    row.ZoomHostEmail = googleHostEmail;
                }
                else if (!storedIsRefreshToken)
                {
                    // No key supplied and none of a connected account to protect: this is
                    // the user clearing the service-account fields.
                    row.ZoomToken = null;
                    row.ZoomHostEmail = googleHostEmail;
                }

                googleRow = row;
            }

            await _context.SaveChangesAsync();
            // The Zoom row is what callers have always been handed back.
            return zoomRow ?? jitsiRow ?? googleRow;
        }

        private async Task<EmployeeRosterLegacy?> UpsertRoster(int employeeId, int? shiftId, int? leaveGroupId, DateTime? startDate, bool isCreate)
        {
            var roster = await _context.EmployeeRosters
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && !(x.UtBlock ?? false) && (x.EndDate == null || x.EndDate.Value.Date == MaxDate.Date));

            if (roster == null && (shiftId.HasValue || leaveGroupId.HasValue))
            {
                var nextId = (await _context.EmployeeRosters.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
                roster = new EmployeeRosterLegacy
                {
                    Id = nextId,
                    EmployeeId = employeeId,
                    ShiftId = shiftId,
                    LeaveGroupId = leaveGroupId,
                    StartDate = (startDate ?? DateTime.UtcNow.Date).Date,
                    EndDate = MaxDate,
                    UtBlock = false
                };
                _context.EmployeeRosters.Add(roster);
                await _context.SaveChangesAsync();
                return roster;
            }

            if (roster != null)
            {
                roster.ShiftId = shiftId ?? roster.ShiftId;
                roster.LeaveGroupId = leaveGroupId;
                if (isCreate && roster.StartDate == null)
                {
                    roster.StartDate = (startDate ?? DateTime.UtcNow.Date).Date;
                }
                if (!isCreate && startDate.HasValue && roster.StartDate == null)
                {
                    roster.StartDate = startDate.Value.Date;
                }
                if (roster.EndDate == null)
                {
                    roster.EndDate = MaxDate;
                }
                await _context.SaveChangesAsync();
            }

            return roster;
        }

        private async Task<EREmpPositionLegacy?> UpsertPosition(int employeeId, int? departmentId, int? designationId, int? gradeId, DateTime? fromDate, string? responsibility, bool isCreate)
        {
            if (!departmentId.HasValue && !designationId.HasValue)
            {
                return null;
            }

            var position = await _context.EmployeePositions
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId == employeeId &&
                    !(x.UtBlock ?? false) &&
                    (!x.TillDate.HasValue || x.TillDate.Value.Date == MaxDate.Date));

            if (position == null)
            {
                var nextId = (await _context.EmployeePositions.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
                position = new EREmpPositionLegacy
                {
                    Id = nextId,
                    EmployeeId = employeeId,
                    DepartmentId = departmentId,
                    DesignationId = designationId,
                    GradeId = gradeId,
                    FromDate = (fromDate ?? DateTime.UtcNow.Date).Date,
                    TillDate = MaxDate,
                    Responsibility = responsibility?.Trim(),
                    UtBlock = false,
                    UtRecordVersion = 1
                };
                _context.EmployeePositions.Add(position);
                await _context.SaveChangesAsync();
                return position;
            }

            position.DepartmentId = departmentId ?? position.DepartmentId;
            position.DesignationId = designationId ?? position.DesignationId;
            position.GradeId = gradeId ?? position.GradeId;
            position.Responsibility = responsibility?.Trim();
            if (isCreate && position.FromDate == null)
            {
                position.FromDate = (fromDate ?? DateTime.UtcNow.Date).Date;
            }
            if (!isCreate && fromDate.HasValue && position.FromDate == null)
            {
                position.FromDate = fromDate.Value.Date;
            }
            if (position.TillDate == null)
            {
                position.TillDate = MaxDate;
            }
            position.UtEditTime = DateTime.UtcNow;
            position.UtRecordVersion = position.UtRecordVersion <= 0 ? 1 : position.UtRecordVersion + 1;
            await _context.SaveChangesAsync();

            return position;
        }

        private async Task<EmployeeAddressLegacy?> UpsertAddress(int employeeId, int? addressTypeId, string? addressText, int? countryId, int? cityId, bool isCreate)
        {
            if (!addressTypeId.HasValue && string.IsNullOrWhiteSpace(addressText) && !countryId.HasValue && !cityId.HasValue)
            {
                return null;
            }

            var address = await _context.EmployeeAddresses
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && !(x.UtBlock ?? false));

            if (address == null)
            {
                var nextId = (await _context.EmployeeAddresses.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
                address = new EmployeeAddressLegacy
                {
                    Id = nextId,
                    EmployeeId = employeeId,
                    AddressTypeId = addressTypeId,
                    Address = addressText?.Trim(),
                    CountryId = countryId,
                    CityId = cityId,
                    PrimaryAddress = true,
                    UtBlock = false,
                    UtBlock1 = false
                };
                _context.EmployeeAddresses.Add(address);
                await _context.SaveChangesAsync();
                return address;
            }

            address.AddressTypeId = addressTypeId ?? address.AddressTypeId;
            address.Address = addressText?.Trim();
            address.CountryId = countryId;
            address.CityId = cityId;
            if (isCreate)
            {
                address.PrimaryAddress = true;
            }
            await _context.SaveChangesAsync();

            return address;
        }

        private async Task<EmployeeSubCoordinateLegacy?> InsertSubCoordinateIfNeeded(
            int employeeId,
            int? subCampusId,
            DateTime? fromDate,
            DateTime? tillDate,
            string? remarks)
        {
            subCampusId = NormalizeId(subCampusId);
            if (!subCampusId.HasValue)
            {
                return null;
            }

            var trimmedRemarks = remarks?.Trim();
            var latest = await _context.EmployeeSubCoordinates
                .Where(x => x.EmployeeId == employeeId && !(x.UtBlock ?? false))
                .OrderByDescending(x => x.FromDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (latest != null &&
                latest.SubCampusId == subCampusId &&
                NullableDateEquals(latest.FromDate, fromDate) &&
                NullableDateEquals(latest.TillDate, tillDate))
            {
                if (!string.Equals(latest.Remarks?.Trim(), trimmedRemarks, StringComparison.Ordinal))
                {
                    latest.Remarks = trimmedRemarks;
                    latest.UtEditTime = DateTime.UtcNow;
                    latest.UtEditUser = GetUserName();
                    latest.UtRecordVersion += 1;
                    await _context.SaveChangesAsync();
                }
                return latest;
            }

            var nextId = (await _context.EmployeeSubCoordinates.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
            var now = DateTime.UtcNow;
            var userName = GetUserName();
            var subCoordinate = new EmployeeSubCoordinateLegacy
            {
                Id = nextId,
                EmployeeId = employeeId,
                SubCampusId = subCampusId,
                FromDate = fromDate?.Date,
                TillDate = tillDate?.Date,
                Remarks = trimmedRemarks,
                UtSysGen = false,
                UtIsPartial = false,
                UtAddTime = now,
                UtAddUser = userName,
                UtEditTime = now,
                UtEditUser = userName,
                UtRecordVersion = 1,
                UtBlock = false
            };

            _context.EmployeeSubCoordinates.Add(subCoordinate);
            await _context.SaveChangesAsync();

            return subCoordinate;
        }

        private static bool NullableDateEquals(DateTime? left, DateTime? right)
        {
            if (!left.HasValue && !right.HasValue) return true;
            if (!left.HasValue || !right.HasValue) return false;
            return left.Value.Date == right.Value.Date;
        }

        private string? GetUserName()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value
                ?? User.Identity?.Name
                ?? Request.Headers["X-User-Name"].FirstOrDefault();
        }

        private async Task<bool> HasAttendanceForEmployee(int employeeId, int clientId, int? entityId)
        {
            if (await _context.EmployeeAttendances.AsNoTracking().AnyAsync(x =>
                x.EmpId == employeeId &&
                x.ClientId == clientId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !x.UtBlock))
            {
                return true;
            }

            if (await _context.DailyAttendances.AsNoTracking().AnyAsync(x =>
                x.EmployeeId == employeeId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !x.UtBlock))
            {
                return true;
            }

            if (await _context.MonthlyAttendances.AsNoTracking().AnyAsync(x =>
                x.EmployeeId == employeeId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !x.UtBlock))
            {
                return true;
            }

            return false;
        }

        private async Task<string?> GenerateEmployeeCodeFromNumberConfigAsync(
            int clientId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_userManagementConnectionString)) return null;

            await using var connection = new SqlConnection(_userManagementConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = """
                select top (1)
                    counterlength,
                    totallength,
                    startvalue,
                    endvalue,
                    allowpadding,
                    paddingvalue
                from utnumberhdr
                where clientid = @clientId
                  and isnull(utblock, 0) = 0
                  and (
                      lower(isnull(fieldid, '')) in ('employeeid', 'employeecode')
                      or lower(isnull(controllerid, '')) like '%employee%'
                      or lower(isnull(modelname, '')) like '%employee%'
                  )
                order by
                    case when lower(isnull(fieldid, '')) in ('employeeid', 'employeecode') then 0 else 1 end,
                    isdefault desc,
                    id desc;
                """;
            command.Parameters.Add(new SqlParameter("@clientId", clientId));

            int? counterLength = null;
            int? totalLength = null;
            int? endValue = null;
            var startValue = 1;
            var allowPadding = false;
            var paddingValue = "0";
            var hasNumberConfig = false;

            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    int? ReadNullableInt(int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
                    counterLength = ReadNullableInt(0);
                    totalLength = ReadNullableInt(1);
                    startValue = ReadNullableInt(2) ?? 1;
                    endValue = ReadNullableInt(3);
                    allowPadding = !reader.IsDBNull(4) && reader.GetBoolean(4);
                    paddingValue = reader.IsDBNull(5) ? "0" : reader.GetString(5);
                    hasNumberConfig = true;
                }
            }

            var existingCodes = await _context.EmployeeProfiles
                .AsNoTracking()
                .Where(x => x.ClientId == clientId && !(x.UtBlock ?? false) && x.EmployeeCode != null)
                .Select(x => x.EmployeeCode!)
                .ToListAsync(cancellationToken);

            var configuredSeries = FindDominantEmployeeCodeSeries(existingCodes);
            if (configuredSeries == null && !hasNumberConfig) return null;

            var existing = configuredSeries?.Codes ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var prefix = configuredSeries?.Prefix ?? string.Empty;
            var next = Math.Max((configuredSeries?.MaxValue ?? 0) + 1, startValue);
            if (endValue.HasValue && next > endValue.Value)
                throw new InvalidOperationException("Employee ID Number Config has reached its End Value.");

            var length = Math.Clamp(counterLength ?? totalLength ?? configuredSeries?.NumberLength ?? 1, 1, 50);
            var padChar = string.IsNullOrWhiteSpace(paddingValue) ? '0' : paddingValue.Trim()[0];
            string Format(int value)
            {
                var code = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var shouldPad = hasNumberConfig ? allowPadding : code.Length < length;
                return shouldPad && code.Length < length ? code.PadLeft(length, padChar) : code;
            }

            var employeeCode = prefix + Format(next);
            while (existing.Contains(employeeCode))
            {
                next++;
                if (endValue.HasValue && next > endValue.Value)
                    throw new InvalidOperationException("Employee ID Number Config has reached its End Value.");
                employeeCode = prefix + Format(next);
            }

            return employeeCode;
        }

        private static EmployeeCodeSeries? FindDominantEmployeeCodeSeries(IEnumerable<string> employeeCodes)
        {
            var matches = employeeCodes
                .Select(code => code?.Trim())
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code =>
                {
                    var lastNonDigit = code!.Length - 1;
                    while (lastNonDigit >= 0 && char.IsDigit(code[lastNonDigit])) lastNonDigit--;

                    var prefix = code[..(lastNonDigit + 1)];
                    var numberText = code[(lastNonDigit + 1)..];
                    return int.TryParse(numberText, out var value)
                        ? new { Code = code, Prefix = prefix, NumberText = numberText, Value = value }
                        : null;
                })
                .Where(match => match != null && !string.IsNullOrWhiteSpace(match.Prefix))
                .Select(match => match!)
                .ToList();

            var group = matches
                .GroupBy(match => match.Prefix, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(item => item.Count())
                .ThenByDescending(item => item.Max(match => match.Value))
                .FirstOrDefault();

            if (group == null) return null;

            var series = group.ToList();
            return new EmployeeCodeSeries(
                series[0].Prefix,
                series.Max(match => match.Value),
                series.Max(match => match.NumberText.Length),
                new HashSet<string>(series.Select(match => match.Code), StringComparer.OrdinalIgnoreCase));
        }

        private sealed record EmployeeCodeSeries(
            string Prefix,
            int MaxValue,
            int NumberLength,
            HashSet<string> Codes);

        private static List<string> ValidateRequest(
            string? firstName,
            int? positionTypeId,
            DateTime? joiningDate,
            int? employeeTypeId,
            int? employeeCategoryId,
            int? courseCategoryId,
            DateTime? dateOfBirth,
            int? departmentId,
            int? designationId,
            int? statusId,
            int? gradeId,
            DateTime? resignDate,
            int? genderId,
            int? birthCountryId,
            int? birthCityId,
            int? nationalityId,
            string? cellNo,
            int? shiftId)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(firstName)) errors.Add("First Name is required.");
            if (!shiftId.HasValue) errors.Add("Shift is required.");
            if (!positionTypeId.HasValue) errors.Add("Position Type is required.");
            if (!joiningDate.HasValue) errors.Add("Joining Date is required.");
            if (!employeeTypeId.HasValue) errors.Add("Employee Type is required.");
            if (!employeeCategoryId.HasValue) errors.Add("Employee Category is required.");
            if (employeeCategoryId == TeacherCategoryId && !courseCategoryId.HasValue)
                errors.Add("Teaching Level is required.");
            if (!dateOfBirth.HasValue) errors.Add("Date of Birth is required.");
            if (dateOfBirth.HasValue && dateOfBirth.Value.Date >= DateTime.UtcNow.Date)
                errors.Add("Date of Birth must be less than current date.");
            if (!departmentId.HasValue) errors.Add("Department is required.");
            if (!designationId.HasValue) errors.Add("Designation is required.");
            if (!statusId.HasValue) errors.Add("Status is required.");
            if (!gradeId.HasValue) errors.Add("Grade is required.");
            if (IsResignedStatus(statusId) && !resignDate.HasValue)
                errors.Add("Leaving Date is required.");
            if (!genderId.HasValue) errors.Add("Gender is required.");
            if (!birthCountryId.HasValue) errors.Add("Birth Country is required.");
            if (!birthCityId.HasValue) errors.Add("Birth City is required.");
            if (!nationalityId.HasValue) errors.Add("Nationality is required.");
            if (string.IsNullOrWhiteSpace(cellNo)) errors.Add("Contact No is required.");

            return errors;
        }

        private static bool IsResignedStatus(int? statusId)
            => statusId.HasValue && ResignedStatusIds.Contains(statusId.Value);

        private Task<EmployeeCreationPolicyV3Dto> ResolveEmployeeCreationPolicyAsync(int clientId, int entityId, CancellationToken cancellationToken) =>
            Task.FromResult(new EmployeeCreationPolicyV3Dto(false, false, true, null));

        private async Task<string?> ValidateReportingToEmployeeAsync(
            int? reportingToEmployeeId,
            int employeeId,
            int clientId,
            int entityId)
        {
            var reportingToId = NormalizeId(reportingToEmployeeId);
            if (!reportingToId.HasValue) return null;

            if (reportingToId.Value == employeeId)
            {
                return "An employee cannot report to themselves.";
            }

            var reportingEmployee = await _context.EmployeeProfiles.AsNoTracking()
                .Where(x =>
                    x.Id == reportingToId.Value &&
                    x.ClientId == clientId &&
                    !(x.UtBlock ?? false))
                .Select(x => new { x.Id, x.EntityId })
                .FirstOrDefaultAsync();

            if (reportingEmployee == null || reportingEmployee.EntityId != entityId)
            {
                return "Reporting To employee is not valid for the selected entity.";
            }

            var visitedEmployeeIds = new HashSet<int>();
            int? currentEmployeeId = reportingEmployee.Id;
            while (currentEmployeeId.HasValue)
            {
                if (currentEmployeeId.Value == employeeId)
                {
                    return "Reporting To selection would create a circular reporting hierarchy.";
                }

                if (!visitedEmployeeIds.Add(currentEmployeeId.Value))
                {
                    return "The selected employee already belongs to a circular reporting hierarchy.";
                }

                currentEmployeeId = await _context.EmployeeProfiles.AsNoTracking()
                    .Where(x =>
                        x.Id == currentEmployeeId.Value &&
                        x.ClientId == clientId &&
                        !(x.UtBlock ?? false))
                    .Select(x => x.ReportingToEmployeeId)
                    .FirstOrDefaultAsync();
            }

            return null;
        }

        private IQueryable<OrgLegalentityLegacy> SubCampusesInScope(int clientId, int parentEntityId) =>
            _context.OrgLegalentities.AsNoTracking().Where(x =>
                x.HeadEntityId == parentEntityId && !x.UtBlock &&
                (!x.ClientId.HasValue || x.ClientId == clientId));

        private async Task<bool> IsSubCampusInScope(int? subCampusId, int clientId, int parentEntityId)
        {
            var selectedId = NormalizeId(subCampusId);
            return !selectedId.HasValue || await SubCampusesInScope(clientId, parentEntityId)
                .AnyAsync(x => x.Id == selectedId.Value);
        }

        private async Task<bool> IsGradeInScope(int? gradeId, int clientId)
        {
            return gradeId.HasValue && await _context.EmployeeGrades.AnyAsync(x =>
                x.Id == gradeId.Value && x.ClientId == clientId && !x.UtBlock);
        }

        /// <summary>
        /// Starts the Google consent flow for one employee. The browser is sent to the
        /// returned URL; Google comes back to the callback below with a code.
        /// </summary>
        [HttpGet("{id:int}/google/connect")]
        public ActionResult<object> StartGoogleConnect(
            int id,
            [FromServices] GoogleOAuthService oauth)
        {
            if (!oauth.IsConfigured)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Google OAuth is not configured on this server. Add GoogleOAuth settings to appsettings.json."
                });
            }

            // The employee id round-trips through Google as the state parameter, so the
            // callback knows whose row to write without trusting anything Google sends.
            return Ok(new { success = true, url = oauth.BuildConsentUrl(id.ToString()) });
        }

        /// <summary>
        /// Where Google returns after the host approves. Anonymous because the browser
        /// arrives straight from Google with no bearer token; the state carries the
        /// employee id and the code is single-use, so nothing here is guessable.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string? code,
            [FromQuery] string? state,
            [FromQuery] string? error,
            [FromServices] GoogleOAuthService oauth,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(error))
                return GoogleCallbackPage(false, $"Google reported: {error}");

            if (string.IsNullOrWhiteSpace(code) || !int.TryParse(state, out var employeeId) || employeeId <= 0)
                return GoogleCallbackPage(false, "The Google response was incomplete. Please try again.");

            var result = await oauth.ExchangeCodeAsync(code, cancellationToken);
            if (!result.Success)
                return GoogleCallbackPage(false, result.Message ?? "Connecting to Google failed.");

            var employee = await _context.EmployeeProfiles.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == employeeId, cancellationToken);
            if (employee == null)
                return GoogleCallbackPage(false, "That employee no longer exists.");

            // Stored exactly where a service-account key would be: same row, same column.
            // Which credential is present decides how the meeting is later created.
            await SaveGoogleCredentialAsync(employeeId, employee.ClientId, result.RefreshToken, result.Email, cancellationToken);

            return GoogleCallbackPage(true, result.Email ?? "your Google account");
        }

        /// <summary>Removes this employee's Google connection, whichever kind it is.</summary>
        [HttpDelete("{id:int}/google/disconnect")]
        public async Task<IActionResult> DisconnectGoogle(int id, CancellationToken cancellationToken)
        {
            var row = await _context.MeetingHostInfos
                .FirstOrDefaultAsync(x => x.EmployeeId == id && x.LovTypeId == LovTypeGoogleMeet && !x.UtBlock, cancellationToken);

            if (row != null)
            {
                row.ZoomToken = null;
                row.ZoomHostEmail = null;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Ok(new { success = true });
        }

        /// <summary>
        /// Writes the refresh token onto the employee's Google row, creating it when the
        /// host has never had one. Mirrors UpsertMeetingHostInfo so the two paths agree
        /// on how a Google row is shaped.
        /// </summary>
        private async Task SaveGoogleCredentialAsync(
            int employeeId,
            int? clientId,
            string? refreshToken,
            string? email,
            CancellationToken cancellationToken)
        {
            var row = await _context.MeetingHostInfos
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.LovTypeId == LovTypeGoogleMeet && !x.UtBlock, cancellationToken);

            if (row == null)
            {
                var nextId = (await _context.MeetingHostInfos.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                row = new MeetingHostInfoLegacy
                {
                    Id = nextId,
                    EmployeeId = employeeId,
                    LovTypeId = LovTypeGoogleMeet,
                    // All four are NOT NULL with no database default.
                    UtSysGen = false,
                    UtBlock = false,
                    UtIsPartial = false,
                    UtRecordVersion = 1
                };
                _context.MeetingHostInfos.Add(row);
            }

            row.ClientId = clientId ?? row.ClientId;
            row.ZoomToken = refreshToken;
            row.ZoomHostEmail = email;
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// A tiny self-closing page, because the callback lands in the popup the tab
        /// opened rather than in the app itself.
        /// </summary>
        private ContentResult GoogleCallbackPage(bool success, string detail)
        {
            var message = success
                ? $"Connected {System.Net.WebUtility.HtmlEncode(detail)}. You can close this window."
                : System.Net.WebUtility.HtmlEncode(detail);
            var colour = success ? "#2e7d32" : "#d32f2f";

            return Content(
                $"<!doctype html><html><head><meta charset=\"utf-8\"><title>Google</title></head>" +
                $"<body style=\"font-family:system-ui;padding:2rem;text-align:center\">" +
                $"<p style=\"color:{colour};font-weight:600\">{message}</p>" +
                "<script>try{window.opener&&window.opener.postMessage({source:'edxs-google-oauth',success:" +
                (success ? "true" : "false") + "},'*');}catch(e){}setTimeout(function(){window.close();},1200);</script>" +
                "</body></html>",
                "text/html");
        }

        private string GetActor() =>
            User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type is "username" or "sub")?.Value ?? "system";

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId))
            {
                return claimClientId;
            }

            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId)
                && int.TryParse(headerClientId, out var parsedHeaderClientId))
            {
                return parsedHeaderClientId;
            }

            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            if (Request.Headers.TryGetValue("X-Campus-Id", out var campusId)
                && int.TryParse(campusId, out var parsedCampusId))
            {
                return parsedCampusId;
            }

            if (Request.Headers.TryGetValue("Campus-Id", out var campusIdAlias)
                && int.TryParse(campusIdAlias, out var parsedCampusIdAlias))
            {
                return parsedCampusIdAlias;
            }

            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId)
                && int.TryParse(headerEntityId, out var parsedHeaderEntityId))
            {
                return parsedHeaderEntityId;
            }

            if (Request.Headers.TryGetValue("Entity-Id", out var entityId)
                && int.TryParse(entityId, out var parsedEntityId))
            {
                return parsedEntityId;
            }

            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId))
            {
                return claimEntityId;
            }

            entityIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "entityId" || c.Type == "entity_id");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out claimEntityId))
            {
                return claimEntityId;
            }

            return null;
        }
    }
}
