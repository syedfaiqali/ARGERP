using EDXS.Shared.Workflow;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;

namespace HumanResource.Service.Workflow;

public sealed class QualificationWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateQualificationV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.Qualification;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetQualificationByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateQualificationV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateQualificationV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateQualificationV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateQualificationV3Command(new(id, r.Code, r.QualificationTypeId, r.Qualification, r.Description, r.LovtypeId, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteQualificationV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateQualificationV3Request r) => $"{r.Code} · {r.Qualification}";
}

public sealed class QualificationTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateQualificationTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.QualificationType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetQualificationTypeByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateQualificationTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateQualificationTypeV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateQualificationTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateQualificationTypeV3Command(new(id, r.Code, r.Name, r.Description, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteQualificationTypeV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateQualificationTypeV3Request r) => $"{r.Code} · {r.Name}";
}

public sealed class SkillWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateSkillV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.Skill;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetSkillByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateSkillV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateSkillV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateSkillV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateSkillV3Command(new(id, r.Code, r.SkillTypeId, r.Name, r.Description, r.RatingModelId, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteSkillV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateSkillV3Request r) => $"{r.Code} · {r.Name}";
}

public sealed class SkillTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateSkillTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.SkillType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetSkillTypeByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateSkillTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateSkillTypeV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateSkillTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateSkillTypeV3Command(new(id, r.Code, r.Name, r.Description, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteSkillTypeV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateSkillTypeV3Request r) => $"{r.Code} · {r.Name}";
}

public sealed class PositionTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreatePositionTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.PositionType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetPositionTypeByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreatePositionTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreatePositionTypeV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreatePositionTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdatePositionTypeV3Command(new(id, r.Code, r.EmployeeTypeId, r.Name, r.Description, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeletePositionTypeV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreatePositionTypeV3Request r) => $"{r.Code} · {r.Name}";
}

public sealed class DocumentTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateDocumentTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.DocumentType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetDocumentTypeByIdV3Query(clientId, entityId ?? 0, id), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateDocumentTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateDocumentTypeV3Command(r.Code, r.DocumentType, r.Description, r.IsActive, clientId, entityId ?? 0), ct);
        return (row, row.Id);
    }
    protected override async Task<bool> UpdateAsync(int id, CreateDocumentTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new UpdateDocumentTypeV3Command(id, r.Code, r.DocumentType, r.Description, r.IsActive, clientId, entityId ?? 0), ct) != null;
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteDocumentTypeV3Command(id, clientId, entityId ?? 0), ct);
    protected override string Subject(CreateDocumentTypeV3Request r) => $"{r.Code} · {r.DocumentType}";
}
