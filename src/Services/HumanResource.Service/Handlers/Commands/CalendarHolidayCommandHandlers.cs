using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Commands
{
    public class CalendarHolidayCommandHandlers :
        IRequestHandler<CreateCalendarHolidayV3Command, CalendarHolidayV3Dto>,
        IRequestHandler<UpdateCalendarHolidayV3Command, CalendarHolidayV3Dto>,
        IRequestHandler<DeleteCalendarHolidayV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public CalendarHolidayCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<CalendarHolidayV3Dto> Handle(CreateCalendarHolidayV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var details = NormalizeDetails(request.Details);
            await ValidateRequestAsync(null, request.CalendarHoliday, request.PayrollPeriodId, request.ApplyOnId, request.SharedWithId,
                request.ApplyOnDetails, request.SharedDetails, details, command.ClientId, command.EntityId, cancellationToken);

            int nextId = (await _context.CalendarHolidays.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            
            var entity = new ETCalholidayhdrLegacy
            {
                Id = nextId,
                Code = string.IsNullOrWhiteSpace(request.Code) ? nextId.ToString("0000") : request.Code.Trim(),
                Stxt = request.CalendarHoliday.Trim(),
                Payperiodid = request.PayrollPeriodId,
                Applyonid = request.ApplyOnId,
                Lovsharedid = request.SharedWithId,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                Utblock = !request.IsActive
            };

            if (details.Count > 0)
            {
                int nextDetailId = (await _context.CalendarHolidayDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                for (int i = 0; i < details.Count; i++)
                {
                    var detail = details[i];
                    entity.Details.Add(new ETCalholidaydtlLegacy
                    {
                        Id = nextDetailId++,
                        Calholidayid = nextId,
                        Lineindex = i + 1,
                        Holidaytypeid = detail.HolidayTypeId,
                        Ltxt = detail.Description,
                        Startdate = detail.StartDate,
                        Enddate = detail.EndDate,
                        Days = detail.Days,
                        Payperioddtlid = detail.PayPeriodDetailId,
                        Utblock = false
                    });
                }
            }

            if (request.ApplyOnDetails != null)
            {
                int nextApplyOnId = (await _context.CalendarHolidayApplyOnDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                for (int i = 0; i < request.ApplyOnDetails.Count; i++)
                {
                    var detail = request.ApplyOnDetails[i];
                    entity.ApplyOnDetails.Add(new ETCalholidayapplyondtlLegacy
                    {
                        Id = nextApplyOnId++,
                        Calholidayid = nextId,
                        Lineindex = i + 1,
                        Departid = detail.DepartId,
                        Emptypeid = detail.EmpTypeId,
                        Utblock = false
                    });
                }
            }

            if (request.SharedDetails != null)
            {
                int nextSharedId = (await _context.CalendarHolidaySharedDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                for (int i = 0; i < request.SharedDetails.Count; i++)
                {
                    var detail = request.SharedDetails[i];
                    entity.SharedDetails.Add(new ETCalholidayshareddtlLegacy
                    {
                        Id = nextSharedId++,
                        Calholidayid = nextId,
                        Lineindex = i + 1,
                        Lovsharedid = detail.SharedWithId,
                        Pclientid = detail.ClientId,
                        Pentityid = detail.EntityId,
                        Regionid = detail.RegionId,
                        Utblock = false
                    });
                }
            }

            _context.CalendarHolidays.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            string payroll = string.Empty;
            if (entity.Payperiodid.HasValue)
                payroll = await _context.PayrollPeriods.Where(p => p.Id == entity.Payperiodid).Select(p => p.Ltxt ?? string.Empty).FirstOrDefaultAsync(cancellationToken) ?? "";

            string applyOn = string.Empty;
            if (entity.Applyonid.HasValue)
                applyOn = await _context.Lovs.Where(l => l.Id == entity.Applyonid).Select(l => l.Stxt ?? string.Empty).FirstOrDefaultAsync(cancellationToken) ?? "";

            string sharedWith = string.Empty;
            if (entity.Lovsharedid.HasValue)
                sharedWith = await _context.Lovs.Where(l => l.Id == entity.Lovsharedid).Select(l => l.Stxt ?? string.Empty).FirstOrDefaultAsync(cancellationToken) ?? "";

            return MapToDto(entity, payroll, applyOn, sharedWith);
        }

        public async Task<CalendarHolidayV3Dto> Handle(UpdateCalendarHolidayV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var details = NormalizeDetails(request.Details);
            await ValidateRequestAsync(request.Id, request.CalendarHoliday, request.PayrollPeriodId, request.ApplyOnId, request.SharedWithId,
                request.ApplyOnDetails, request.SharedDetails, details, command.ClientId, command.EntityId, cancellationToken);

            var entity = await _context.CalendarHolidays
                .Include(x => x.Details)
                .Include(x => x.ApplyOnDetails)
                .Include(x => x.SharedDetails)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value || x.EntityId == null), cancellationToken);

            if (entity == null) throw new InvalidOperationException($"Calendar Holiday not found.");

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                entity.Code = request.Code.Trim();
            }
            entity.Stxt = request.CalendarHoliday.Trim();
            entity.Payperiodid = request.PayrollPeriodId;
            entity.Applyonid = request.ApplyOnId;
            entity.Lovsharedid = request.SharedWithId;
            entity.Utblock = !request.IsActive;

            // Details
            _context.CalendarHolidayDetails.RemoveRange(entity.Details);
            if (details.Count > 0)
            {
                int nextDetailId = (await _context.CalendarHolidayDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                for (int i = 0; i < details.Count; i++)
                {
                    var detail = details[i];
                    entity.Details.Add(new ETCalholidaydtlLegacy
                    {
                        Id = detail.Id.HasValue && detail.Id.Value > 0 ? detail.Id.Value : nextDetailId++,
                        Calholidayid = entity.Id,
                        Lineindex = i + 1,
                        Holidaytypeid = detail.HolidayTypeId,
                        Ltxt = detail.Description,
                        Startdate = detail.StartDate,
                        Enddate = detail.EndDate,
                        Days = detail.Days,
                        Payperioddtlid = detail.PayPeriodDetailId,
                        Utblock = false
                    });
                }
            }

            // Apply On
            _context.CalendarHolidayApplyOnDetails.RemoveRange(entity.ApplyOnDetails);
            if (request.ApplyOnDetails != null)
            {
                int nextApplyOnId = (await _context.CalendarHolidayApplyOnDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                for (int i = 0; i < request.ApplyOnDetails.Count; i++)
                {
                    var detail = request.ApplyOnDetails[i];
                    entity.ApplyOnDetails.Add(new ETCalholidayapplyondtlLegacy
                    {
                        Id = detail.Id ?? nextApplyOnId++,
                        Calholidayid = entity.Id,
                        Lineindex = i + 1,
                        Departid = detail.DepartId,
                        Emptypeid = detail.EmpTypeId,
                        Utblock = false
                    });
                }
            }

            // Shared
            _context.CalendarHolidaySharedDetails.RemoveRange(entity.SharedDetails);
            if (request.SharedDetails != null)
            {
                int nextSharedId = (await _context.CalendarHolidaySharedDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                for (int i = 0; i < request.SharedDetails.Count; i++)
                {
                    var detail = request.SharedDetails[i];
                    entity.SharedDetails.Add(new ETCalholidayshareddtlLegacy
                    {
                        Id = detail.Id ?? nextSharedId++,
                        Calholidayid = entity.Id,
                        Lineindex = i + 1,
                        Lovsharedid = detail.SharedWithId,
                        Pclientid = detail.ClientId,
                        Pentityid = detail.EntityId,
                        Regionid = detail.RegionId,
                        Utblock = false
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            string payroll = string.Empty;
            if (entity.Payperiodid.HasValue)
                payroll = await _context.PayrollPeriods.Where(p => p.Id == entity.Payperiodid).Select(p => p.Ltxt ?? string.Empty).FirstOrDefaultAsync(cancellationToken) ?? "";

            string applyOn = string.Empty;
            if (entity.Applyonid.HasValue)
                applyOn = await _context.Lovs.Where(l => l.Id == entity.Applyonid).Select(l => l.Stxt ?? string.Empty).FirstOrDefaultAsync(cancellationToken) ?? "";

            string sharedWith = string.Empty;
            if (entity.Lovsharedid.HasValue)
                sharedWith = await _context.Lovs.Where(l => l.Id == entity.Lovsharedid).Select(l => l.Stxt ?? string.Empty).FirstOrDefaultAsync(cancellationToken) ?? "";

            return MapToDto(entity, payroll, applyOn, sharedWith);
        }

        private static List<CalendarHolidayDetailSaveModel> NormalizeDetails(IEnumerable<CreateCalendarHolidayDetailV3Request>? details)
        {
            return details?.Select(d => new CalendarHolidayDetailSaveModel(
                null,
                d.HolidayTypeId,
                d.Description?.Trim() ?? string.Empty,
                d.StartDate,
                d.EndDate,
                d.Days,
                d.PayPeriodDetailId
            )).ToList() ?? new List<CalendarHolidayDetailSaveModel>();
        }

        private static List<CalendarHolidayDetailSaveModel> NormalizeDetails(IEnumerable<UpdateCalendarHolidayDetailV3Request>? details)
        {
            return details?.Select(d => new CalendarHolidayDetailSaveModel(
                d.Id,
                d.HolidayTypeId,
                d.Description?.Trim() ?? string.Empty,
                d.StartDate,
                d.EndDate,
                d.Days,
                d.PayPeriodDetailId
            )).ToList() ?? new List<CalendarHolidayDetailSaveModel>();
        }

        private async Task ValidateRequestAsync<TApplyOn, TShared>(
            int? calendarHolidayId, string? calendarHoliday, int? payrollPeriodId, int? applyOnId, int? sharedWithId,
            IEnumerable<TApplyOn>? applyOnDetails, IEnumerable<TShared>? sharedDetails,
            List<CalendarHolidayDetailSaveModel> details, int clientId, int? entityId, CancellationToken cancellationToken)
            where TApplyOn : class where TShared : class
        {
            if (string.IsNullOrWhiteSpace(calendarHoliday)) throw new InvalidOperationException("Calendar Holiday cannot be empty.");
            if (!payrollPeriodId.HasValue) throw new InvalidOperationException("Payroll Period cannot be empty.");
            if (!applyOnId.HasValue) throw new InvalidOperationException("Apply On cannot be empty.");
            if (!sharedWithId.HasValue) throw new InvalidOperationException("Shared With cannot be empty.");

            var payrollExists = await _context.PayrollPeriods.AsNoTracking().AnyAsync(p =>
                p.Id == payrollPeriodId.Value && p.ClientId == clientId && !p.Utblock &&
                (!entityId.HasValue || p.EntityId == entityId.Value || p.EntityId == null), cancellationToken);
            if (!payrollExists) throw new InvalidOperationException("Payroll Period does not exist for the current entity.");

            var normalizedName = calendarHoliday.Trim();
            var duplicateName = await _context.CalendarHolidays.AsNoTracking().AnyAsync(x =>
                x.ClientId == clientId && x.EntityId == entityId && !x.Utblock && x.Id != calendarHolidayId && x.Stxt == normalizedName,
                cancellationToken);
            if (duplicateName) throw new InvalidOperationException("Calendar Holiday already exists.");

            var applyRows = applyOnDetails?.Cast<object>().ToList() ?? new List<object>();
            if (applyRows.Count == 0) throw new InvalidOperationException("Apply On Detail(s) cannot be empty.");
            var sharedRows = sharedDetails?.Cast<object>().ToList() ?? new List<object>();
            if (sharedRows.Count == 0) throw new InvalidOperationException("Shared With Detail(s) cannot be empty.");

            const int departmentApplyOn = 584;
            const int employeeTypeApplyOn = 585;
            const int entitySharedWith = 520;
            if (applyOnId != departmentApplyOn && applyOnId != employeeTypeApplyOn)
                throw new InvalidOperationException("Selected Apply On is not supported.");
            if (sharedWithId != entitySharedWith)
                throw new InvalidOperationException("Calendar Holiday can only be shared with Entity.");

            foreach (var row in applyRows)
            {
                var departId = (int?)row.GetType().GetProperty("DepartId")?.GetValue(row);
                var empTypeId = (int?)row.GetType().GetProperty("EmpTypeId")?.GetValue(row);
                if ((applyOnId == departmentApplyOn && (!departId.HasValue || empTypeId.HasValue)) ||
                    (applyOnId == employeeTypeApplyOn && (!empTypeId.HasValue || departId.HasValue)))
                    throw new InvalidOperationException("Apply On Detail does not match the selected Apply On.");
            }

            var departmentIds = applyRows
                .Select(row => (int?)row.GetType().GetProperty("DepartId")?.GetValue(row))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();
            if (departmentIds.Count > 0 && await _context.Departments.AsNoTracking().CountAsync(x => departmentIds.Contains(x.Id), cancellationToken) != departmentIds.Count)
                throw new InvalidOperationException("Apply On Department does not exist.");

            var employeeTypeIds = applyRows
                .Select(row => (int?)row.GetType().GetProperty("EmpTypeId")?.GetValue(row))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();
            if (employeeTypeIds.Count > 0 && await _context.Lovs.AsNoTracking().CountAsync(x =>
                employeeTypeIds.Contains(x.Id) && x.Vtype == "EMPTP" && !x.UtBlock, cancellationToken) != employeeTypeIds.Count)
                throw new InvalidOperationException("Apply On Employee Type does not exist.");

            foreach (var row in sharedRows)
            {
                var rowSharedWithId = (int?)row.GetType().GetProperty("SharedWithId")?.GetValue(row);
                var sharedEntityId = (int?)row.GetType().GetProperty("EntityId")?.GetValue(row);
                if (rowSharedWithId != entitySharedWith || !sharedEntityId.HasValue)
                    throw new InvalidOperationException("Shared With Detail must contain an Entity.");
                var entityExists = await _context.OrgLegalentities.AsNoTracking().AnyAsync(x =>
                    x.Id == sharedEntityId.Value && x.ClientId == clientId && !x.UtBlock, cancellationToken);
                if (!entityExists) throw new InvalidOperationException("Shared entity does not exist for the current client.");
            }

            await ValidateDetailsAsync(details, payrollPeriodId.Value, cancellationToken);
        }

        private async Task ValidateDetailsAsync(List<CalendarHolidayDetailSaveModel> details, int payrollPeriodId, CancellationToken cancellationToken)
        {
            if (details.Count == 0)
            {
                throw new InvalidOperationException("Holiday Details cannot be empty.");
            }

            var periodIds = details
                .Where(d => d.PayPeriodDetailId.HasValue)
                .Select(d => d.PayPeriodDetailId!.Value)
                .Distinct()
                .ToList();

            var periods = await _context.PayrollPeriodDetails
                .AsNoTracking()
                .Where(p => periodIds.Contains(p.Id) && p.Payperiodid == payrollPeriodId && !p.utblock)
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            var holidayTypeIds = details.Where(d => d.HolidayTypeId.HasValue).Select(d => d.HolidayTypeId!.Value).Distinct().ToList();
            var validHolidayTypeIds = await _context.HolidayTypes.AsNoTracking()
                .Where(x => holidayTypeIds.Contains(x.Id) && !(x.UtBlock ?? false))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            for (int i = 0; i < details.Count; i++)
            {
                var detail = details[i];
                var lineId = $"Line ID : {i + 1} ";

                if (!detail.HolidayTypeId.HasValue)
                {
                    throw new InvalidOperationException($"{lineId}Holiday Type cannot be empty.");
                }
                else if (!validHolidayTypeIds.Contains(detail.HolidayTypeId.Value))
                {
                    throw new InvalidOperationException($"{lineId}Holiday Type does not exist.");
                }

                if (string.IsNullOrWhiteSpace(detail.Description))
                {
                    throw new InvalidOperationException($"{lineId}Description cannot be empty.");
                }

                if (!detail.StartDate.HasValue)
                {
                    throw new InvalidOperationException($"{lineId}Start Date cannot be empty.");
                }

                if (!detail.EndDate.HasValue)
                {
                    throw new InvalidOperationException($"{lineId}End Date cannot be empty.");
                }

                var startDate = detail.StartDate.Value.Date;
                var endDate = detail.EndDate.Value.Date;

                if (startDate > endDate)
                {
                    throw new InvalidOperationException($"{lineId}Start Date cannot be greater than End Date.");
                }

                detail.Days = (endDate - startDate).Days + 1;

                if (!detail.PayPeriodDetailId.HasValue || !periods.TryGetValue(detail.PayPeriodDetailId.Value, out var period))
                {
                    throw new InvalidOperationException($"{lineId}Payroll Period Detail does not exist.");
                }

                if (!period.Startdate.HasValue || !period.Enddate.HasValue)
                {
                    throw new InvalidOperationException($"{lineId}Selected Period date range is not defined.");
                }

                var periodStart = period.Startdate.Value.Date;
                var periodEnd = period.Enddate.Value.Date;
                if (startDate < periodStart || startDate > periodEnd)
                {
                    throw new InvalidOperationException($"{lineId}Start Date is not valid against selected Period.");
                }

                if (endDate < periodStart || endDate > periodEnd)
                {
                    throw new InvalidOperationException($"{lineId}End Date is not valid against selected Period.");
                }
            }

            for (int i = 0; i < details.Count; i++)
            {
                var current = details[i];
                var startDate = current.StartDate!.Value.Date;
                var endDate = current.EndDate!.Value.Date;

                var duplicateDescription = details
                    .Take(i)
                    .Any(previous => previous.PayPeriodDetailId == current.PayPeriodDetailId
                        && previous.HolidayTypeId == current.HolidayTypeId
                        && previous.Description.Equals(current.Description, StringComparison.OrdinalIgnoreCase));

                if (duplicateDescription)
                {
                    throw new InvalidOperationException($"Line ID : {i + 1} Description already exists against Holiday Type.");
                }

                var overlapIndex = details.Take(i).ToList().FindIndex(previous =>
                    previous.PayPeriodDetailId == current.PayPeriodDetailId
                    && startDate <= previous.EndDate!.Value.Date
                    && endDate >= previous.StartDate!.Value.Date);

                if (overlapIndex >= 0)
                {
                    throw new InvalidOperationException($"Line ID : {i + 1} Date Range is conflicting with Line ID : {overlapIndex + 1}.");
                }
            }
        }

        private sealed class CalendarHolidayDetailSaveModel
        {
            public CalendarHolidayDetailSaveModel(int? id, int? holidayTypeId, string description, DateTime? startDate, DateTime? endDate, int? days, int? payPeriodDetailId)
            {
                Id = id;
                HolidayTypeId = holidayTypeId;
                Description = description;
                StartDate = startDate;
                EndDate = endDate;
                Days = days;
                PayPeriodDetailId = payPeriodDetailId;
            }

            public int? Id { get; }
            public int? HolidayTypeId { get; }
            public string Description { get; }
            public DateTime? StartDate { get; }
            public DateTime? EndDate { get; }
            public int? Days { get; set; }
            public int? PayPeriodDetailId { get; }
        }

        public async Task<bool> Handle(DeleteCalendarHolidayV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.CalendarHolidays
                .Include(x => x.Details)
                .Include(x => x.ApplyOnDetails)
                .Include(x => x.SharedDetails)
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value || x.EntityId == null), cancellationToken);

            if (entity == null) return false;

            entity.Utblock = true;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private CalendarHolidayV3Dto MapToDto(ETCalholidayhdrLegacy entity, string payrollPeriod = "", string applyOn = "", string sharedWith = "")
        {
            // 1. Bulk fetch all needed period details to avoid N+1
            var ppDtlIds = entity.Details
                .Where(d => d.Payperioddtlid.HasValue)
                .Select(d => d.Payperioddtlid!.Value)
                .Distinct()
                .ToList();

            var ppDtls = _context.PayrollPeriodDetails
                .Where(pd => ppDtlIds.Contains(pd.Id))
                .ToList();

            // 2. Map
            return new CalendarHolidayV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Payperiodid,
                entity.Applyonid,
                entity.Lovsharedid,
                payrollPeriod,
                applyOn,
                sharedWith,
                !entity.Utblock,
                entity.Details
                    .OrderBy(d => d.Startdate ?? DateTime.MaxValue)
                    .ThenBy(d => d.Id)
                    .Select(d => {
                    var ppDtl = d.Payperioddtlid.HasValue 
                        ? ppDtls.FirstOrDefault(p => p.Id == d.Payperioddtlid.Value) 
                        : null;
                    return new CalendarHolidayDetailV3Dto(
                        d.Id,
                        d.Holidaytypeid,
                        d.Ltxt ?? string.Empty,
                        d.Startdate,
                        d.Enddate,
                        d.Days,
                        d.Payperioddtlid,
                        ppDtl?.Year,
                        ppDtl?.Startdate.HasValue == true ? ppDtl.Startdate.Value.ToString("MMMM") : (ppDtl?.Periodstxt ?? string.Empty)
                    );
                    }).ToList(),
                entity.ApplyOnDetails.Select(a => new CalendarHolidayApplyOnDetailV3Dto(
                    a.Id,
                    a.Departid,
                    a.Emptypeid
                )).ToList(),
                entity.SharedDetails.Select(s => new CalendarHolidaySharedDetailV3Dto(
                    s.Id,
                    s.Lovsharedid,
                    s.Pclientid,
                    s.Pentityid,
                    s.Regionid
                )).ToList()
            );
        }
    }
}
