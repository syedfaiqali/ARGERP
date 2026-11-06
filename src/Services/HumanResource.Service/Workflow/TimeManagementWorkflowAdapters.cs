using EDXS.Shared.Workflow;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;

namespace HumanResource.Service.Workflow;

public sealed class HolidayTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateHolidayTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.HolidayType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetHolidayTypeByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateHolidayTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateHolidayTypeV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateHolidayTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateHolidayTypeV3Command(new(id, r.Code, r.HolidayType, r.Description, r.ColorCode, r.IsActive), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteHolidayTypeV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateHolidayTypeV3Request r) => $"{r.Code} · {r.HolidayType}";
}

public sealed class AttendanceActivityWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateAttendanceActivityV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.AttendanceActivity;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetAttendanceActivityByIdV3Query(id, clientId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateAttendanceActivityV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateAttendanceActivityV3Command(r, clientId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateAttendanceActivityV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateAttendanceActivityV3Command(new(id, r.OrderId, r.Activity, r.Value), clientId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteAttendanceActivityV3Command(id, clientId), ct);
    protected override string Subject(CreateAttendanceActivityV3Request r) => r.Activity ?? "Attendance Activity";
}

public sealed class DeviceTypeWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateDeviceTypeV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.DeviceType;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetDeviceTypeByIdV3Query(id, clientId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateDeviceTypeV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateDeviceTypeV3Command(r, clientId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateDeviceTypeV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateDeviceTypeV3Command(new(id, r.Code, r.Name, r.Description, r.IsActive), clientId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteDeviceTypeV3Command(id, clientId), ct);
    protected override string Subject(CreateDeviceTypeV3Request r) => $"{r.Code} · {r.Name}";
}

public sealed class DeviceRegistrationWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<DeviceRegistrationUpsertV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.DeviceRegistration;
    // The device registration queries take a campus id where 0 means "no campus".
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetDeviceRegistrationByIdV3Query(id, clientId, entityId ?? 0), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(DeviceRegistrationUpsertV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateDeviceRegistrationV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override async Task<bool> UpdateAsync(int id, DeviceRegistrationUpsertV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new UpdateDeviceRegistrationV3Command(id, r, clientId, entityId), ct) != null;
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteDeviceRegistrationV3Command(id, clientId, entityId), ct);
    protected override string Subject(DeviceRegistrationUpsertV3Request r) => $"{r.Code} · {r.DeviceName}";
}

public sealed class AttendancePolicyWorkflowAdapter(HumanResourceDbContext db, IMediator mediator) : HrSetupWorkflowAdapter<CreateAttendancePolicyV3Request>(db)
{
    public override ActionRegistration Registration => PilotActions.AttendancePolicy;
    protected override async Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        await mediator.Send(new GetAttendancePolicyByIdV3Query(id, clientId, entityId), ct);
    protected override async Task<(object Row, int Id)> CreateAsync(CreateAttendancePolicyV3Request r, int clientId, int? entityId, CancellationToken ct)
    {
        var row = await mediator.Send(new CreateAttendancePolicyV3Command(r, clientId, entityId), ct);
        return (row, row.Id);
    }
    protected override Task<bool> UpdateAsync(int id, CreateAttendancePolicyV3Request r, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new UpdateAttendancePolicyV3Command(new(id, r.Code, r.Stxt, r.PolicyTypeId, r.ApplyOffDays, r.NoOfExcuse, r.IsActive,
            r.PolicyDetails, r.AbsentDetails), clientId, entityId), ct);
    protected override Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct) =>
        mediator.Send(new DeleteAttendancePolicyV3Command(id, clientId, entityId), ct);
    protected override string Subject(CreateAttendancePolicyV3Request r) => string.IsNullOrWhiteSpace(r.Code) ? r.Stxt : $"{r.Code} · {r.Stxt}";
}
