using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ARGERP.Auth;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Infrastructure.Data;

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
var local = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(root, ".local/settings.json")));
if (args.Contains("--inspect-zkt"))
{
    await ZkSourceSetup.InspectAsync(root);
    return;
}
var connectionString = local.RootElement.GetProperty("connectionString").GetString()!;
if (args.Contains("--test-zkt")) { await ZkSyncIntegrationTest.RunAsync(connectionString); return; }
var connectionBuilder = new SqlConnectionStringBuilder(connectionString);
if (!connectionBuilder.InitialCatalog.Equals("ARGERP_HR", StringComparison.Ordinal)) throw new InvalidOperationException("Setup only operates on the separate ARGERP_HR database.");
var masterBuilder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
await using (var master = new SqlConnection(masterBuilder.ConnectionString))
{
    await master.OpenAsync();
    if (args.Contains("--recreate-empty"))
    {
        var existing = new SqlConnection(connectionString);
        await existing.OpenAsync();
        await using var check = new SqlCommand("SELECT (SELECT COUNT(*) FROM dbo.EREmployeeinfohdr) + (SELECT COUNT(*) FROM dbo.EPLoanrequest);", existing);
        if (Convert.ToInt32(await check.ExecuteScalarAsync()) != 0) throw new InvalidOperationException("Refusing to recreate a database containing HR records.");
        await existing.DisposeAsync(); SqlConnection.ClearAllPools();
        await using var reset = new SqlCommand("ALTER DATABASE [ARGERP_HR] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [ARGERP_HR];", master);
        await reset.ExecuteNonQueryAsync();
    }
    await using var command = new SqlCommand("IF DB_ID(N'ARGERP_HR') IS NULL CREATE DATABASE [ARGERP_HR];", master);
    await command.ExecuteNonQueryAsync();
}
await using var hr = new HumanResourceDbContext(new DbContextOptionsBuilder<HumanResourceDbContext>().UseSqlServer(connectionString).Options);
await using var organization = new OrganizationDbContext(new DbContextOptionsBuilder<OrganizationDbContext>().UseSqlServer(connectionString).Options);
await using var auth = new ArgAuthDbContext(new DbContextOptionsBuilder<ArgAuthDbContext>().UseSqlServer(connectionString).Options);
var schemas = new[] { hr.Database.GenerateCreateScript(), organization.Database.GenerateCreateScript(), auth.Database.GenerateCreateScript() };
await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
var output = new StringBuilder("-- ARGERP HR standalone schema. Generated from the checked-in EF models.\n-- Run against ARGERP_HR only. Existing EDXS databases are never used.\n");
foreach (var schema in schemas)
{
    foreach (var batch in Regex.Split(schema, @"^GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase).Where(x => !string.IsNullOrWhiteSpace(x)))
    {
        var create = Regex.Match(batch, @"CREATE TABLE (?<table>(?:\[[^\]]+\]\.)?\[[^\]]+\])\s*\((?<body>[\s\S]*)\);", RegexOptions.IgnoreCase);
        if (!create.Success) continue;
        var table = create.Groups["table"].Value;
        var body = create.Groups["body"].Value;
        // Relationships are added after all contexts' shared master tables exist.
        body = Regex.Replace(body, @",?\s*CONSTRAINT \[[^\]]+\] FOREIGN KEY[^\r\n]*", "", RegexOptions.IgnoreCase);
        var statement = $"IF OBJECT_ID(N'{table}', N'U') IS NULL BEGIN CREATE TABLE {table} ({body}); END;";
        await Execute(statement);
        foreach (Match column in Regex.Matches(body, @"^\s*\[(?<name>[^\]]+)\]\s+(?<definition>[^\r\n]+)", RegexOptions.Multiline))
        {
            var name = column.Groups["name"].Value;
            var definition = column.Groups["definition"].Value.TrimEnd().TrimEnd(',');
            // A shared master may have extra columns in the other context. Add these without rewriting data.
            definition = Regex.Replace(definition, @"NOT NULL", "NULL", RegexOptions.IgnoreCase);
            definition = Regex.Replace(definition, @"IDENTITY(?:\([^)]*\))?", "", RegexOptions.IgnoreCase);
            await Execute($"IF COL_LENGTH(N'{table}', N'{name}') IS NULL ALTER TABLE {table} ADD [{name}] {definition};");
        }
    }
}
foreach (var schema in schemas)
foreach (Match foreign in Regex.Matches(schema, @"CREATE TABLE (?<table>(?:\[[^\]]+\]\.)?\[[^\]]+\])\s*\((?<body>[\s\S]*?)\);", RegexOptions.IgnoreCase))
foreach (Match constraint in Regex.Matches(foreign.Groups["body"].Value, @"CONSTRAINT \[(?<name>[^\]]+)\] FOREIGN KEY[^\r\n]*", RegexOptions.IgnoreCase))
{
    var definition = constraint.Value.TrimEnd().TrimEnd(',');
    await Execute($"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'{constraint.Groups["name"].Value}') ALTER TABLE {foreign.Groups["table"].Value} ADD {definition};");
}
foreach (var schema in schemas)
foreach (Match index in Regex.Matches(schema, @"CREATE (?:UNIQUE )?INDEX \[(?<name>[^\]]+)\] ON (?<table>(?:\[[^\]]+\]\.)?\[[^\]]+\])[^;]*;", RegexOptions.IgnoreCase))
    await Execute($"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'{index.Groups["name"].Value}' AND object_id=OBJECT_ID(N'{index.Groups["table"].Value}')) {index.Value}");

var extra = Path.Combine(root, "database/002-support.sql");
foreach (var batch in Regex.Split(await File.ReadAllTextAsync(extra), @"^GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase).Where(x => !string.IsNullOrWhiteSpace(x))) await Execute(batch);
var demo = Path.Combine(root, "database/003-demo.sql");
foreach (var batch in Regex.Split(await File.ReadAllTextAsync(demo), @"^GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase).Where(x => !string.IsNullOrWhiteSpace(x))) await Execute(batch);
if (!await auth.Users.AnyAsync())
{
    var user = new ArgUser { Username = "admin", FullName = "ARG ERP Administrator", EntityId = 1, ClientId = 1, EmployeeId = 1 };
    user.PasswordHash = new PasswordHasher<ArgUser>().HashPassword(user, local.RootElement.GetProperty("adminPassword").GetString()!);
    auth.Users.Add(user); await auth.SaveChangesAsync();
}
if (args.Contains("--import-zkt-mappings")) await LegacyZkMappings.ImportAsync(root, connectionString);
Directory.CreateDirectory(Path.Combine(root, "database"));
await File.WriteAllTextAsync(Path.Combine(root, "database/001-schema.generated.sql"), output.ToString());
Console.WriteLine("ARGERP_HR schema and administrator are ready. No EDXS database was changed.");

async Task Execute(string sql)
{
    await using var command = new SqlCommand(sql, connection) { CommandTimeout = 180 };
    try { await command.ExecuteNonQueryAsync(); }
    catch (Exception) { Console.Error.WriteLine("Failed schema statement: " + sql[..Math.Min(sql.Length, 300)]); throw; }
    output.AppendLine(sql).AppendLine("GO");
}
