using HumanResource.Service.Entities;

namespace HumanResource.Service.Services;

internal static class VacancyUtMetadata
{
    private const string SystemName = "HR.Service";

    public static void Initialize(
        IVacancyUtTracked entity,
        string actor,
        bool systemGenerated,
        DateTime? timestamp = null)
    {
        var now = timestamp ?? DateTime.UtcNow;
        var user = ShortActor(actor);

        entity.UtDocType = 0;
        entity.UtSubDocType = 0;
        entity.UtSysGen = systemGenerated;
        entity.UtBlock = false;
        entity.UtIsPartial = false;
        entity.UtAddTime = now;
        entity.UtAddUser = user;
        entity.UtAddSystem = SystemName;
        entity.UtAddIp = string.Empty;
        entity.UtAddAppVersion = string.Empty;
        entity.UtAddDbVersion = string.Empty;
        entity.UtAddId = 0;
        entity.UtAddSessionId = 0;
        entity.UtEditTime = now;
        entity.UtEditUser = user;
        entity.UtEditSystem = SystemName;
        entity.UtEditIp = string.Empty;
        entity.UtEditAppVersion = string.Empty;
        entity.UtEditDbVersion = string.Empty;
        entity.UtEditId = 0;
        entity.UtEditSessionId = 0;
        entity.UtRecordVersion = 1;
    }

    public static void Touch(IVacancyUtTracked entity, string actor, DateTime? timestamp = null)
    {
        entity.UtEditTime = timestamp ?? DateTime.UtcNow;
        entity.UtEditUser = ShortActor(actor);
        entity.UtEditSystem = SystemName;
        entity.UtEditIp = string.Empty;
        entity.UtEditAppVersion = string.Empty;
        entity.UtEditDbVersion = string.Empty;
        entity.UtEditId = 0;
        entity.UtEditSessionId = 0;
        entity.UtRecordVersion = Math.Max(entity.UtRecordVersion + 1, 1);
    }

    private static string ShortActor(string actor)
    {
        var value = string.IsNullOrWhiteSpace(actor) ? "system" : actor.Trim();
        return value[..Math.Min(value.Length, 25)];
    }
}
