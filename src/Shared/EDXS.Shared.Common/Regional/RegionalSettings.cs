using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Text.Json;

namespace EDXS.Shared.Common.Regional;

public sealed record RegionalSettings(int CountryId, string CountryCode, string CurrencyCode,
    string CurrencyName, string CurrencySymbol, string? TimeZone, bool CurrencyOverridden,
    IReadOnlyList<string> TimeZones)
{
    public int DecimalPlaces { get; init; } = 2;
}

/// <summary>Reads only regional defaults. Never changes transaction amounts, dates or currencies.</summary>
public static class RegionalSettingsStore
{
    public static async Task<RegionalSettings?> ReadAsync(DbConnection connection, int clientId,
        int entityId, CancellationToken token = default, DbTransaction? transaction = null)
    {
        if (clientId <= 0 || entityId <= 0) return null;
        var opened = connection.State != ConnectionState.Open;
        try
        {
            if (opened) await connection.OpenAsync(token);
            // Separate schema probe: SQL Server can compile references to absent tables even
            // inside an IF. An older installation keeps its existing behavior until migrated.
            await using var probe = connection.CreateCommand();
            probe.Transaction = transaction;
            probe.CommandText = "SELECT CASE WHEN OBJECT_ID('dbo.SMRegionalCountry') IS NOT NULL AND OBJECT_ID('dbo.SMRegionalPolicy') IS NOT NULL AND OBJECT_ID('dbo.SMRegionalLocation') IS NOT NULL THEN 1 ELSE 0 END";
            if (Convert.ToInt32(await probe.ExecuteScalarAsync(token)) != 1) return null;
            probe.CommandText = "SELECT CASE WHEN COL_LENGTH('dbo.SMRegionalPolicy','decimalplaces') IS NULL THEN 0 ELSE 1 END";
            var hasDecimalPlaces = Convert.ToInt32(await probe.ExecuteScalarAsync(token)) == 1;
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = """
                SELECT c.countryid, c.iso2, COALESCE(o.currencycode,c.currencycode),
                    COALESCE(o.currencyname,c.currencyname), COALESCE(o.currencysymbol,c.currencysymbol),
                    COALESCE(o.timezone,cityzone.timezone,statezone.timezone),
                    CASE WHEN o.currencycode IS NULL THEN 0 ELSE 1 END, c.timezones, DECIMAL_PLACES
                FROM dbo.SMPolicy p
                JOIN dbo.SMRegionalCountry c ON c.countryid=p.defaultctryid AND c.utblock=0
                LEFT JOIN dbo.SMRegionalPolicy o ON o.clientid=p.clientid AND o.entityid=p.entityid AND o.utblock=0
                LEFT JOIN dbo.SMRegionalLocation cityzone ON cityzone.kind='city' AND cityzone.localid=p.defaultcityid AND cityzone.utblock=0
                LEFT JOIN dbo.SMRegionalLocation statezone ON statezone.kind='state' AND statezone.localid=p.defaultstateid AND statezone.utblock=0
                WHERE p.clientid=@client AND p.entityid=@entity AND ISNULL(p.utblock,0)=0
                ORDER BY p.Id
                """.Replace("DECIMAL_PLACES", hasDecimalPlaces ? "COALESCE(o.decimalplaces,2)" : "CAST(2 AS int)");
            Add(command, "@client", clientId); Add(command, "@entity", entityId);
            await using var reader = await command.ExecuteReaderAsync(token);
            if (!await reader.ReadAsync(token)) return null;
            var zones = JsonSerializer.Deserialize<string[]>(reader.GetString(7)) ?? [];
            var zone = reader.IsDBNull(5) ? (zones.Length == 1 ? zones[0] : null) : reader.GetString(5);
            return new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                reader.GetString(3), reader.GetString(4), zone, reader.GetInt32(6) == 1, zones)
                { DecimalPlaces = Math.Clamp(Convert.ToInt32(reader.GetValue(8)), 0, 6) };
        }
        finally { if (opened && connection.State == ConnectionState.Open) await connection.CloseAsync(); }
    }

    public static bool IsValidTimeZone(string? zone)
    {
        if (string.IsNullOrWhiteSpace(zone)) return false;
        try { _ = TimeZoneInfo.FindSystemTimeZoneById(zone); return true; }
        catch (TimeZoneNotFoundException) { return false; }
        catch (InvalidTimeZoneException) { return false; }
    }

    public static async Task WriteCurrencyOverrideAsync(DbConnection connection, int client, int entity,
        int id, string code, string name, string symbol, CancellationToken token, DbTransaction? transaction = null)
    {
        if (client <= 0 || entity <= 0) throw new InvalidOperationException("Client and entity are required.");
        var opened = connection.State != ConnectionState.Open;
        try
        {
            if (opened) await connection.OpenAsync(token);
            await using var probe = connection.CreateCommand(); probe.Transaction = transaction;
            probe.CommandText = "SELECT CASE WHEN OBJECT_ID('dbo.SMRegionalPolicy') IS NULL THEN 0 ELSE 1 END";
            if (Convert.ToInt32(await probe.ExecuteScalarAsync(token)) == 0) return;
            await using var command = connection.CreateCommand(); command.Transaction = transaction;
            command.CommandText = """
                UPDATE dbo.SMRegionalPolicy WITH(UPDLOCK,HOLDLOCK) SET currencyid=@id,currencycode=@code,
                    currencyname=@name,currencysymbol=@symbol,utrecordversion=utrecordversion+1
                    WHERE clientid=@client AND entityid=@entity;
                IF @@ROWCOUNT=0 INSERT dbo.SMRegionalPolicy(clientid,entityid,currencyid,currencycode,currencyname,currencysymbol)
                    VALUES(@client,@entity,@id,@code,@name,@symbol);
                """;
            Add(command,"@client",client); Add(command,"@entity",entity); Add(command,"@id",id);
            Add(command,"@code",code); Add(command,"@name",name); Add(command,"@symbol",symbol);
            await command.ExecuteNonQueryAsync(token);
        }
        finally { if (opened && connection.State == ConnectionState.Open) await connection.CloseAsync(); }
    }

    public static DateTime NowInZone(string? zone, DateTime fallback)
        => IsValidTimeZone(zone) ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById(zone!)) : fallback;

    private static readonly ConcurrentDictionary<(int Client, int Entity), (string? Zone, DateTime ExpiresUtc)> ZoneCache = new();

    /// <summary>
    /// Wall-clock time of the campus' System Policy timezone, so "today" does not depend on the
    /// server's own timezone (containers run in UTC). The zone is cached for a few minutes because
    /// attendance writes call this per row. Falls back to the server clock when no zone is
    /// configured or the regional tables cannot be read.
    /// </summary>
    public static async Task<DateTime> NowAsync(DbConnection connection, int clientId, int entityId,
        CancellationToken token = default, DbTransaction? transaction = null)
    {
        var key = (clientId, entityId);
        if (!ZoneCache.TryGetValue(key, out var cached) || cached.ExpiresUtc < DateTime.UtcNow)
        {
            try { cached = ((await ReadAsync(connection, clientId, entityId, token, transaction))?.TimeZone, DateTime.UtcNow.AddMinutes(5)); }
            catch (DbException) { return DateTime.Now; }
            ZoneCache[key] = cached;
        }
        return NowInZone(cached.Zone, DateTime.Now);
    }

    public static void Add(DbCommand command, string name, object? value)
    {
        var p = command.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value;
        command.Parameters.Add(p);
    }
}
