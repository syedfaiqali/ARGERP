using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace HumanResource.Service.Controllers;
[ApiController, Authorize, Route("api/Reference/v3/{kind}")]
public sealed class ReferenceDataV3Controller(HumanResourceDbContext db) : ControllerBase
{
    private sealed record Mapping(string Table, string Code, string Name, string? Description, bool Client, bool Entity);
    private static readonly Dictionary<string, Mapping> Tables = new(StringComparer.OrdinalIgnoreCase)
    {
        ["classes"] = new("SMCourse", "code", "stxt", null, true, true),
        ["teaching-subjects"] = new("SMSubject", "code", "stxt", null, true, true),
        ["religions"] = new("religion", "religioncode", "stxt", "ltxt", false, false),
        ["banks"] = new("FMBankgrp", "code", "stxt", "ltxt", true, true),
        ["relations"] = new("SMRelationtype", "relationtypeno", "stxt", "ltxt", true, false),
        ["diseases"] = new("SMDisease", "code", "stxt", "ltxt", true, true),
        ["employment-types"] = new("EREmploymenttype", "code", "stxt", "ltxt", false, false),
        ["income-tax"] = new("orgincomtaxhdr", "code", "stxt", null, true, false),
    };
    public sealed record Row(int Id, string? Code, string? Name, string? Description, bool Inactive);
    private int Entity => int.TryParse(Request.Headers["X-Entity-Id"], out var id) ? id : 1;
    private Mapping Map(string kind) => Tables.TryGetValue(kind, out var map) ? map : throw new ArgumentException("Unknown reference list.");
    private string Scope(Mapping map) => (map.Client ? " AND clientid=1" : "") + (map.Entity ? " AND (entityid=@entity OR entityid IS NULL)" : "");
    [HttpGet]
    public async Task<IActionResult> Get(string kind, CancellationToken ct)
    {
        var m = Map(kind);
        var sql = $"SELECT Id, [{m.Code}] Code, [{m.Name}] Name, {(m.Description == null ? "CAST(NULL AS nvarchar(500))" : $"[{m.Description}]")} Description, CAST(ISNULL(utblock,0) AS bit) Inactive FROM [{m.Table}] WHERE 1=1 {Scope(m)} ORDER BY [{m.Name}]";
        return Ok(await db.Database.SqlQueryRaw<Row>(sql, new SqlParameter("@entity", Entity)).ToListAsync(ct));
    }
    [HttpPost]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Save(string kind, int? id, Row row, CancellationToken ct)
    {
        var m = Map(kind);
        if (string.IsNullOrWhiteSpace(row.Code) || row.Code.Length > 10 || string.IsNullOrWhiteSpace(row.Name) || row.Name.Length > 150 || row.Description?.Length > 500) return BadRequest(new { error = "Code (up to 10 characters) and name (up to 150 characters) are required. Description may contain up to 500 characters." });
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        await db.Database.OpenConnectionAsync(ct);
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = tx.GetDbTransaction();
        command.CommandText = "DECLARE @r int; EXEC @r=sp_getapplock @Resource=@resource,@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=15000; IF @r<0 THROW 51000,'Reference list is busy. Please retry.',1;";
        command.Parameters.Add(new SqlParameter("@resource", "arg-reference-" + m.Table));
        await command.ExecuteNonQueryAsync(ct);
        command.Parameters.Clear();
        foreach (var parameter in new[] { new SqlParameter("@id", id ?? 0), new SqlParameter("@code", row.Code.Trim()), new SqlParameter("@name", row.Name.Trim()), new SqlParameter("@description", (object?)row.Description?.Trim() ?? DBNull.Value), new SqlParameter("@inactive", row.Inactive), new SqlParameter("@entity", Entity) }) command.Parameters.Add(parameter);
        command.CommandText = $"SELECT COUNT(*) FROM [{m.Table}] WHERE Id<>@id AND ISNULL(utblock,0)=0 AND ([{m.Code}]=@code OR [{m.Name}]=@name) {Scope(m)};";
        if (Convert.ToInt32(await command.ExecuteScalarAsync(ct)) > 0) return Conflict(new { error = "A record with this code or name already exists." });
        if (id.HasValue)
        {
            command.CommandText = $"UPDATE [{m.Table}] SET [{m.Code}]=@code,[{m.Name}]=@name,utblock=@inactive{(m.Description == null ? "" : $",[{m.Description}]=@description")} WHERE Id=@id {Scope(m)};";
            if (await command.ExecuteNonQueryAsync(ct) == 0) return NotFound();
        }
        else
        {
            var columns = $"[{m.Code}],[{m.Name}],utblock" + (m.Description == null ? "" : $",[{m.Description}]") + (m.Client ? ",clientid" : "") + (m.Entity ? ",entityid" : "");
            var values = "@code,@name,@inactive" + (m.Description == null ? "" : ",@description") + (m.Client ? ",1" : "") + (m.Entity ? ",@entity" : "");
            command.CommandText = $"IF COLUMNPROPERTY(OBJECT_ID(N'{m.Table}'),'Id','IsIdentity')=1 BEGIN INSERT [{m.Table}] ({columns}) VALUES({values}); END ELSE BEGIN DECLARE @next int=(SELECT ISNULL(MAX(Id),0)+1 FROM [{m.Table}]); INSERT [{m.Table}] (Id,{columns}) VALUES(@next,{values}); END;";
            await command.ExecuteNonQueryAsync(ct);
        }
        await tx.CommitAsync(ct); return Ok(new { success = true });
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(string kind, int id, CancellationToken ct)
    {
        var m = Map(kind);
        var count = await db.Database.ExecuteSqlRawAsync($"UPDATE [{m.Table}] SET utblock=1 WHERE Id=@id {Scope(m)}", [new SqlParameter("@id", id), new SqlParameter("@entity", Entity)], ct);
        return count == 0 ? NotFound() : NoContent();
    }
}
