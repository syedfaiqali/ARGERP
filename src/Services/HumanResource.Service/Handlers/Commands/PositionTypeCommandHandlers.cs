using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class PositionTypeCommandHandlers :
        IRequestHandler<CreatePositionTypeV3Command, PositionTypeV3Dto>,
        IRequestHandler<UpdatePositionTypeV3Command, bool>,
        IRequestHandler<DeletePositionTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public PositionTypeCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PositionTypeV3Dto> Handle(CreatePositionTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequiredFields(request.Code, request.EmployeeTypeId, request.Name);

            if (await _context.PositionTypes.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Position Type with code '{request.Code}' already exists.");
            }

            var nextId = (await _context.PositionTypes.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new ERPositiontypeLegacy
            {
                Id = nextId,
                Code = request.Code.Trim(),
                EmpTypeId = request.EmployeeTypeId,
                Stxt = request.Name.Trim(),
                Ltxt = request.Description?.Trim() ?? string.Empty,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtBlock = !request.IsActive
            };

            _context.PositionTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var etName = await _context.EmploymentTypes.Where(x => x.Id == entity.EmpTypeId).Select(x => x.Stxt).FirstOrDefaultAsync(cancellationToken);

            return new PositionTypeV3Dto(entity.Id, entity.Code ?? string.Empty, entity.EmpTypeId, etName, entity.Stxt ?? string.Empty, entity.Ltxt, !(entity.UtBlock ?? false));
        }

        public async Task<bool> Handle(UpdatePositionTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            ValidateRequiredFields(request.Code, request.EmployeeTypeId, request.Name);
            var entity = await _context.PositionTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            if (await _context.PositionTypes.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another position type with code '{request.Code}' already exists.");
            }

            entity.Code = request.Code.Trim();
            entity.EmpTypeId = request.EmployeeTypeId;
            entity.Stxt = request.Name.Trim();
            entity.Ltxt = request.Description?.Trim() ?? string.Empty;
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static void ValidateRequiredFields(string? code, int? employeeTypeId, string? name)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Code is required.");
            if (!employeeTypeId.HasValue)
                throw new InvalidOperationException("Employee Type is required.");
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Position Type is required.");
        }

        public async Task<bool> Handle(DeletePositionTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.PositionTypes
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            _context.PositionTypes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
