using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class EmploymentCommandHandlers :
        IRequestHandler<CreateEmploymentV3Command, EmploymentV3Dto>,
        IRequestHandler<UpdateEmploymentV3Command, bool>,
        IRequestHandler<DeleteEmploymentV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public EmploymentCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<EmploymentV3Dto> Handle(CreateEmploymentV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequest(request);

            var nextId = (await _context.EmployeeEmployments.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new EmployeeEmploymentLegacy
            {
                Id = nextId,
                ClientId = command.ClientId,
                EntityId = command.EntityId,
                EmployeeId = request.EmployeeId,
                EmploymentTypeId = request.EmploymentTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UtSysGen = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtRecordVersion = 0,
                UtBlock = false
            };

            _context.EmployeeEmployments.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var typeName = await _context.EmploymentTypes
                .Where(x => x.Id == entity.EmploymentTypeId)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            return new EmploymentV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.EmploymentTypeId,
                typeName ?? string.Empty,
                entity.StartDate,
                entity.EndDate,
                !entity.UtBlock
            );
        }

        public async Task<bool> Handle(UpdateEmploymentV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            if (!request.Id.HasValue)
            {
                throw new InvalidOperationException("Employment Id is required for update.");
            }

            ValidateRequest(request);

            var entity = await _context.EmployeeEmployments
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id.Value
                        && x.ClientId == command.ClientId
                        && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value),
                    cancellationToken
                );

            if (entity == null) return false;
            if (entity.UtSysGen)
            {
                throw new InvalidOperationException("Employment history created from Employee Profile cannot be edited here.");
            }

            entity.EmployeeId = request.EmployeeId;
            entity.EmploymentTypeId = request.EmploymentTypeId;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.UtEditTime = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteEmploymentV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.EmployeeEmployments
                .FirstOrDefaultAsync(
                    x => x.Id == command.Id
                        && x.ClientId == command.ClientId
                        && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value),
                    cancellationToken
                );

            if (entity == null) return false;
            if (entity.UtSysGen)
            {
                throw new InvalidOperationException("Employment history created from Employee Profile cannot be deleted.");
            }

            _context.EmployeeEmployments.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static void ValidateRequest(EmploymentUpsertV3Request request)
        {
            if (!request.EmployeeId.HasValue || request.EmployeeId.Value <= 0)
            {
                throw new InvalidOperationException("Employee is required.");
            }

            if (!request.EmploymentTypeId.HasValue || request.EmploymentTypeId.Value <= 0)
            {
                throw new InvalidOperationException("Employment Type is required.");
            }

            if (!request.StartDate.HasValue)
            {
                throw new InvalidOperationException("Start Date is required.");
            }
        }

    }
}
