using System.Text.Json;
using Microsoft.Data.SqlClient;

internal static class ZkSourceSetup
{
    public static async Task InspectAsync(string root)
    {
        using var source = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(root, ".local/zkt-source.json")));
        foreach (var key in new[] { "zkConnectionString", "organizationConnectionString" })
        {
            var builder = new SqlConnectionStringBuilder(source.RootElement.GetProperty(key).GetString()) { ConnectTimeout = 8, ApplicationIntent = ApplicationIntent.ReadOnly };
            try
            {
                await using var connection = new SqlConnection(builder.ConnectionString);
                await connection.OpenAsync();
                var sql = key == "zkConnectionString"
                    ? "SELECT 'terminals', COUNT_BIG(*) FROM iclock_terminal UNION ALL SELECT 'punches', COUNT_BIG(*) FROM iclock_transaction; SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME IN ('iclock_terminal','iclock_transaction','personnel_employee') ORDER BY TABLE_NAME,ORDINAL_POSITION;"
                    : "SELECT 'machines', COUNT_BIG(*) FROM SMMachineRegistration UNION ALL SELECT 'mappedEmployees', COUNT_BIG(*) FROM EREmployeeinfohdr WHERE NULLIF(machinecode,'') IS NOT NULL;";
                await using var command = new SqlCommand(sql, connection) { CommandTimeout = 20 };
                await using var reader = await command.ExecuteReaderAsync();
                do { while (await reader.ReadAsync()) Console.WriteLine(key + ": " + string.Join(" | ", Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetValue(i).ToString()))); } while (await reader.NextResultAsync());
            }
            catch (SqlException e) { Console.WriteLine(key + ": unavailable (SQL error " + e.Number + "). No source database changed."); }
        }
    }
}
