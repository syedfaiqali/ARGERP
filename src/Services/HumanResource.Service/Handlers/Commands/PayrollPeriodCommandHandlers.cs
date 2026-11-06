using HumanResource.Service.Commands;
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
    public class PayrollPeriodCommandHandlers :
        IRequestHandler<CreatePayrollPeriodV3Command, int>,
        IRequestHandler<UpdatePayrollPeriodV3Command, bool>,
        IRequestHandler<DeletePayrollPeriodV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public PayrollPeriodCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreatePayrollPeriodV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var nextId = (await _context.PayrollPeriods.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var code = string.IsNullOrWhiteSpace(request.Code)
                ? nextId.ToString("0000")
                : request.Code.Trim();

            var entity = new ETPayperiodhdrLegacy
            {
                Id = nextId,
                Code = code,
                Date = request.Date ?? DateTime.Now,
                Atntypeid = request.Atntypeid,
                Fromyear = request.Fromyear,
                Toyear = request.Toyear,
                Ltxt = request.Ltxt,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                Startfrom = request.Startfrom,
                Statusid = request.Statusid ?? await GetOpenStatusIdAsync(cancellationToken),
                Lovsharedid = request.Lovsharedid,
                Startdate = request.Startdate,
                Payrollregisterid = request.Payrollregisterid,
                Noofperiod = request.Noofperiod,
                utsysgen = false,
                utispartial = false,
                utrecordversion = 1,
                Utblock = !request.IsActive
            };

            var nextDetailId = (await _context.PayrollPeriodDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            foreach (var dtl in request.Details)
            {
                entity.Details.Add(new ETPayperioddtlLegacy
                {
                    Id = nextDetailId++,
                    Payperiodid = entity.Id,
                    Year = dtl.Year,
                    Monthid = dtl.StartDate?.Month,
                    Periodstxt = dtl.Periodstxt,
                    Startdate = dtl.StartDate,
                    Enddate = dtl.EndDate,
                    Noofdays = dtl.NoOfDays,
                    Disbdate = dtl.DisbDate,
                    Statusid = dtl.StatusId,
                    Lineindex = dtl.LineIndex,
                    utsysgen = false,
                    utispartial = false,
                    utrecordversion = 1,
                    utblock = false
                });
            }

            _context.PayrollPeriods.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task<bool> Handle(UpdatePayrollPeriodV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.PayrollPeriods
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null) return false;

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                entity.Code = request.Code.Trim();
            }
            entity.Date = request.Date;
            entity.Atntypeid = request.Atntypeid;
            entity.Fromyear = request.Fromyear;
            entity.Toyear = request.Toyear;
            entity.Ltxt = request.Ltxt;
            entity.Startfrom = request.Startfrom;
            entity.Statusid = request.Statusid ?? await GetOpenStatusIdAsync(cancellationToken);
            entity.Lovsharedid = request.Lovsharedid;
            entity.Startdate = request.Startdate;
            entity.Payrollregisterid = request.Payrollregisterid;
            entity.Noofperiod = request.Noofperiod;
            entity.utsysgen = false;
            entity.utispartial = false;
            entity.utrecordversion = entity.utrecordversion <= 0 ? 1 : entity.utrecordversion + 1;
            entity.Utblock = !request.IsActive;

            // Simple detail management: Clear and re-add (common in this legacy pattern if IDs are not tracked strictly)
            // Or better, track by ID
            var existingDetails = entity.Details.ToList();
            _context.PayrollPeriodDetails.RemoveRange(existingDetails);

            var nextDetailId = (await _context.PayrollPeriodDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            foreach (var dtl in request.Details)
            {
                entity.Details.Add(new ETPayperioddtlLegacy
                {
                    Id = dtl.Id ?? nextDetailId++,
                    Payperiodid = entity.Id,
                    Year = dtl.Year,
                    Monthid = dtl.StartDate?.Month,
                    Periodstxt = dtl.Periodstxt,
                    Startdate = dtl.StartDate,
                    Enddate = dtl.EndDate,
                    Noofdays = dtl.NoOfDays,
                    Disbdate = dtl.DisbDate,
                    Statusid = dtl.StatusId,
                    Lineindex = dtl.LineIndex,
                    utsysgen = false,
                    utispartial = false,
                    utrecordversion = 1,
                    utblock = false
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task<int?> GetOpenStatusIdAsync(CancellationToken cancellationToken)
        {
            return await _context.Lovs
                .Where(x => !x.UtBlock && x.Vtype == "STATS" &&
                    (x.Code == "OPN" || x.Stxt == "Open"))
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> Handle(DeletePayrollPeriodV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.PayrollPeriods.FindAsync(new object[] { command.Id }, cancellationToken);
            if (entity == null) return false;

            entity.Utblock = true; // Soft delete
            entity.utrecordversion = entity.utrecordversion <= 0 ? 1 : entity.utrecordversion + 1;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
