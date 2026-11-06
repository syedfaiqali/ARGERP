using Microsoft.EntityFrameworkCore.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class QualificationTypeCommandHandlers :
        IRequestHandler<CreateQualificationTypeV3Command, QualificationTypeV3Dto>,
        IRequestHandler<UpdateQualificationTypeV3Command, bool>,
        IRequestHandler<DeleteQualificationTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;
        private const int HumanResourceLovTypeId = 368;

        public QualificationTypeCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<QualificationTypeV3Dto> Handle(CreateQualificationTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (await _context.QualificationTypes.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Qualification Type with code '{request.Code}' already exists.");
            }

            var nextId = (await _context.QualificationTypes.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new ERQualificationtypeLegacy
            {
                Id = nextId,
                Code = request.Code.Trim(),
                Stxt = request.Name.Trim(),
                Ltxt = request.Description?.Trim() ?? string.Empty,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                LovtypeId = HumanResourceLovTypeId,
                UtBlock = !request.IsActive
            };

            _context.QualificationTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new QualificationTypeV3Dto(entity.Id, entity.Code ?? string.Empty, entity.Stxt ?? string.Empty, entity.Ltxt, !(entity.UtBlock ?? false));
        }

        public async Task<bool> Handle(UpdateQualificationTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.QualificationTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            if (await _context.QualificationTypes.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another qualification type with code '{request.Code}' already exists.");
            }

            entity.Code = request.Code.Trim();
            entity.Stxt = request.Name.Trim();
            entity.Ltxt = request.Description?.Trim() ?? string.Empty;
            entity.UtBlock = !request.IsActive;
            entity.LovtypeId = HumanResourceLovTypeId;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteQualificationTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.QualificationTypes
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            var usedInQualification = await _context.Qualifications
                .AnyAsync(x => x.quatypeid == entity.Id && !(x.utblock ?? false), cancellationToken);
            var connection = _context.Database.GetDbConnection();
            var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;
            if (shouldCloseConnection) await connection.OpenAsync(cancellationToken);
            await using var dbCommand = connection.CreateCommand();
            dbCommand.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            dbCommand.CommandText = "select case when exists (select 1 from SMAcademic where qualificationtypeid = @qualificationTypeId) then 1 else 0 end";
            var parameter = dbCommand.CreateParameter();
            parameter.ParameterName = "@qualificationTypeId";
            parameter.Value = entity.Id;
            dbCommand.Parameters.Add(parameter);
            var usedInAcademic = Convert.ToInt32(await dbCommand.ExecuteScalarAsync(cancellationToken)) == 1;
            if (shouldCloseConnection) await connection.CloseAsync();
            if (usedInQualification || usedInAcademic)
            {
                throw new InvalidOperationException("Qualification Type cannot be deleted because it is used in Qualification.");
            }

            _context.QualificationTypes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

