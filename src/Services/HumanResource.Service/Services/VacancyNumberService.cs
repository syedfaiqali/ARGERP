using HumanResource.Service.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Services;

public interface IVacancyNumberService
{
    Task<string> NextVacancyNumberAsync(int clientId, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> NextPositionNumbersAsync(int clientId, int count, CancellationToken cancellationToken);
}

public sealed class VacancyNumberService : IVacancyNumberService
{
    private readonly HumanResourceDbContext _context;
    private readonly string? _userManagementConnectionString;

    public VacancyNumberService(HumanResourceDbContext context, IConfiguration configuration)
    {
        _context = context;
        _userManagementConnectionString = configuration.GetConnectionString("UserManagementDB");
    }

    public async Task<string> NextVacancyNumberAsync(int clientId, CancellationToken cancellationToken)
    {
        var settings = await LoadSettingsAsync(clientId, "vacancynumber", "vacancy", cancellationToken);
        var existing = await _context.Vacancies.AsNoTracking()
            .Where(x => x.ClientId == clientId)
            .Select(x => x.VacancyNumber)
            .ToListAsync(cancellationToken);
        return Next("VAC-", existing, settings);
    }

    public async Task<IReadOnlyList<string>> NextPositionNumbersAsync(int clientId, int count, CancellationToken cancellationToken)
    {
        var settings = await LoadSettingsAsync(clientId, "positionnumber", "vacancyposition", cancellationToken);
        var existing = await _context.VacancyPositions.AsNoTracking()
            .Join(_context.Vacancies, position => position.VacancyId, vacancy => vacancy.Id, (position, vacancy) => new { position, vacancy })
            .Where(x => x.vacancy.ClientId == clientId)
            .Select(x => x.position.PositionNumber)
            .ToListAsync(cancellationToken);

        var result = new List<string>(count);
        for (var index = 0; index < count; index++)
        {
            var value = Next("POS-", existing.Concat(result), settings);
            result.Add(value);
        }
        return result;
    }

    private async Task<NumberSettings> LoadSettingsAsync(
        int clientId,
        string fieldName,
        string modelName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_userManagementConnectionString)) return NumberSettings.Default;

        await using var connection = new SqlConnection(_userManagementConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            select top (1) counterlength, totallength, startvalue, endvalue, allowpadding, paddingvalue
            from utnumberhdr
            where clientid = @clientId
              and isnull(utblock, 0) = 0
              and (lower(isnull(fieldid, '')) = @fieldName
                   or lower(isnull(modelname, '')) like '%' + @modelName + '%')
            order by case when lower(isnull(fieldid, '')) = @fieldName then 0 else 1 end,
                     isdefault desc, id desc;
            """;
        command.Parameters.Add(new SqlParameter("@clientId", clientId));
        command.Parameters.Add(new SqlParameter("@fieldName", fieldName));
        command.Parameters.Add(new SqlParameter("@modelName", modelName));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return NumberSettings.Default;

        int? ReadInt(int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        var length = Math.Clamp(ReadInt(0) ?? ReadInt(1) ?? 6, 1, 20);
        var start = Math.Max(ReadInt(2) ?? 1, 1);
        var end = ReadInt(3);
        var pad = !reader.IsDBNull(4) && reader.GetBoolean(4);
        var paddingText = reader.IsDBNull(5) ? "0" : reader.GetString(5);
        return new NumberSettings(length, start, end, pad, string.IsNullOrWhiteSpace(paddingText) ? '0' : paddingText.Trim()[0]);
    }

    private static string Next(string prefix, IEnumerable<string> existingCodes, NumberSettings settings)
    {
        var maximum = existingCodes
            .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(x => int.TryParse(x[prefix.Length..], out var number) ? number : 0)
            .DefaultIfEmpty(0)
            .Max();
        var next = Math.Max(maximum + 1, settings.Start);
        if (settings.End.HasValue && next > settings.End.Value)
            throw new InvalidOperationException($"{prefix.TrimEnd('-')} Number Config has reached its End Value.");

        var numberText = next.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (settings.AllowPadding && numberText.Length < settings.Length)
            numberText = numberText.PadLeft(settings.Length, settings.PaddingCharacter);
        else if (numberText.Length < settings.Length)
            numberText = numberText.PadLeft(settings.Length, '0');
        return prefix + numberText;
    }

    private sealed record NumberSettings(int Length, int Start, int? End, bool AllowPadding, char PaddingCharacter)
    {
        public static NumberSettings Default { get; } = new(6, 1, null, true, '0');
    }
}
