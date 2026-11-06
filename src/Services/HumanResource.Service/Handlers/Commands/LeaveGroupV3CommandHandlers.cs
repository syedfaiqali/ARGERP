using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class LeaveGroupV3CommandHandlers : 
        IRequestHandler<CreateLeaveGroupV3Command, int>,
        IRequestHandler<UpdateLeaveGroupV3Command, bool>,
        IRequestHandler<DeleteLeaveGroupV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public LeaveGroupV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateLeaveGroupV3Command command, CancellationToken cancellationToken)
        {
            var dto = command.Dto;
            var lastId = await _context.LeaveGroups.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;
            var lastDtlId = await _context.LeaveGroupDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;

            var entity = new LeaveGroupLegacy
            {
                Id = lastId + 1,
                Code = dto.Code,
                Stxt = dto.Stxt,
                AdjustmentPolicyId = dto.AdjustmentPolicyId,
                ClientId = command.ClientId,
                EntityId = command.EntityId,
                UtBlock = false,
                UtSysGen = true,
                UtIsPartial = false,
                UtRecordVersion = 1
            };

            foreach (var dtlDto in dto.Details)
            {
                lastDtlId++;
                entity.Details.Add(new LeaveGroupDetailLegacy
                {
                    Id = lastDtlId,
                    LeaveTypeId = dtlDto.LeaveTypeId,
                    LeaveDays = dtlDto.LeaveDays,
                    UtBlock = false,
                    UtSysGen = true,
                    UtRecordVersion = 1
                });
            }

            await _context.LeaveGroups.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task<bool> Handle(UpdateLeaveGroupV3Command command, CancellationToken cancellationToken)
        {
            var dto = command.Dto;
            var entity = await _context.LeaveGroups
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

            if (entity == null) return false;

            entity.Code = dto.Code;
            entity.Stxt = dto.Stxt;
            entity.AdjustmentPolicyId = dto.AdjustmentPolicyId;
            entity.UtRecordVersion++;

            // Handle Details
            // Remove missing
            var detailsToRemove = entity.Details.Where(d => !dto.Details.Any(dd => dd.Id == d.Id && d.Id > 0)).ToList();
            foreach (var d in detailsToRemove)
            {
                _context.LeaveGroupDetails.Remove(d);
            }

            // Update existing or add new
            var lastDtlId = await _context.LeaveGroupDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;
            foreach (var dtlDto in dto.Details)
            {
                if (dtlDto.Id > 0)
                {
                    var existing = entity.Details.FirstOrDefault(d => d.Id == dtlDto.Id);
                    if (existing != null)
                    {
                        existing.LeaveTypeId = dtlDto.LeaveTypeId;
                        existing.LeaveDays = dtlDto.LeaveDays;
                        existing.UtRecordVersion++;
                    }
                }
                else
                {
                    lastDtlId++;
                    entity.Details.Add(new LeaveGroupDetailLegacy
                    {
                        Id = lastDtlId,
                        LeaveTypeId = dtlDto.LeaveTypeId,
                        LeaveDays = dtlDto.LeaveDays,
                        UtBlock = false,
                        UtSysGen = true,
                        UtRecordVersion = 1
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteLeaveGroupV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.LeaveGroups
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (entity == null) return false;

            // Details will be deleted by cascade or manually
            _context.LeaveGroupDetails.RemoveRange(entity.Details);
            _context.LeaveGroups.Remove(entity);
            
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
