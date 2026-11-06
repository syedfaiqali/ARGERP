using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;

// Copies only attendance identity, device and shift references. Source connections execute SELECT only.
internal sealed class LegacyZkMappings
{
    private readonly SqlConnection source;
    private readonly SqlConnection target;
    private readonly SqlTransaction transaction;
    private readonly string sourceKey;
    private readonly Dictionary<string, Dictionary<int, int>> maps = new();
    private LegacyZkMappings(SqlConnection source, SqlConnection target, SqlTransaction transaction)
    {
        this.source = source; this.target = target; this.transaction = transaction;
        var cs = new SqlConnectionStringBuilder(source.ConnectionString);
        sourceKey = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cs.DataSource.ToLowerInvariant() + "|" + cs.InitialCatalog.ToLowerInvariant())));
    }
    public static async Task ImportAsync(string root, string targetConnection)
    {
        if (new SqlConnectionStringBuilder(targetConnection).InitialCatalog != "ARGERP_HR") throw new InvalidOperationException("Mapping import only targets ARGERP_HR.");
        using var settings = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(root, ".local/zkt-source.json")));
        var builder = new SqlConnectionStringBuilder(settings.RootElement.GetProperty("organizationConnectionString").GetString()) { ApplicationIntent = ApplicationIntent.ReadOnly, ConnectTimeout = 10 };
        await using var source = new SqlConnection(builder.ConnectionString);
        await using var target = new SqlConnection(targetConnection);
        await source.OpenAsync(); await target.OpenAsync();
        await using var transaction = (SqlTransaction)await target.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var importer = new LegacyZkMappings(source, target, transaction);
        await importer.RunAsync();
        await transaction.CommitAsync();
    }
    private async Task RunAsync()
    {
        await Execute("IF OBJECT_ID('dbo.ARG_LegacyMappings','U') IS NULL CREATE TABLE dbo.ARG_LegacyMappings(SourceKey varchar(64) NOT NULL, Kind varchar(40) NOT NULL, LegacyId int NOT NULL, TargetId int NOT NULL, PRIMARY KEY(SourceKey,Kind,LegacyId));");
        foreach (var kind in new[] { "OrgLegalentity", "OrgDepartment", "OrgDesignation", "ETShifthdr", "ETShiftdtl", "EREmployeeinfohdr", "SMMachineRegistration", "lov" })
        {
            maps[kind] = new();
            await using var command = Command("SELECT LegacyId,TargetId FROM dbo.ARG_LegacyMappings WHERE SourceKey=@source AND Kind=@kind", ("@source",sourceKey),("@kind",kind));
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) maps[kind][reader.GetInt32(0)] = reader.GetInt32(1);
        }
        var employees = await Read("EREmployeeinfohdr", "Id,clientid,entityid,employeecode,firstname,lastname,depid,designationid,empstatusid,shiftid,machinecode,joiningdate,resigndate,utblock", "WHERE NULLIF(LTRIM(RTRIM(machinecode)),'') IS NOT NULL");
        var machines = await Read("SMMachineRegistration", "Id,clientid,entityid,code,stxt,ltxt,machineip,port,inactive,utblock");
        var branches = employees.Concat(machines).Select(x => Int(x,"entityid")).Where(x=>x.HasValue).Select(x=>x!.Value).ToHashSet();
        foreach (var row in await Read("OrgLegalentity", "Id,stxt,entityno,shortname,campusname"))
            if (branches.Contains(Int(row,"Id")!.Value)) await Add("OrgLegalentity",row, new() { ["clientid"]=1 });
        var departments = employees.Select(x=>Int(x,"depid")).ToHashSet();
        foreach (var row in await Read("OrgDepartment", "Id,depcode,depstxt"))
            if (departments.Contains(Int(row,"Id"))) await Add("OrgDepartment", row, new() { ["clientid"]=1 });
        var designations = employees.Select(x=>Int(x,"designationid")).ToHashSet();
        foreach (var row in await Read("OrgDesignation", "Id,desgcode,stxt"))
            if (designations.Contains(Int(row,"Id"))) await Add("OrgDesignation",row,new() { ["clientid"]=1 });
        var statuses = employees.Select(x=>Int(x,"empstatusid")).ToHashSet();
        foreach (var row in await Read("lov", "id,code,stxt,ltxt,vtype"))
            if (statuses.Contains(Int(row,"id"))) await Add("lov", row, new());
        var shifts = employees.Select(x=>Int(x,"shiftid")).ToHashSet();
        foreach (var row in await Read("ETShifthdr", "Id,code,stxt,entityid,womonday,wotuesday,wowednesday,wothuresday,wofriday,wosaturday,wosunday,utblock"))
            if (shifts.Contains(Int(row,"Id"))) await Add("ETShifthdr", row, new() { ["clientid"]=1,["entityid"]=Mapped("OrgLegalentity",Int(row,"entityid")) });
        foreach (var row in await Read("ETShiftdtl", "Id,shiftid,lineindex,stxt,dayid,intime,outtime,gracetime,gracetimeend,latepolicyapplicable,earlypolicyapplicable,startdate,endate,duration,utblock"))
            if (shifts.Contains(Int(row,"shiftid"))) await Add("ETShiftdtl",row,new() { ["shiftid"]=Mapped("ETShifthdr",Int(row,"shiftid")) });
        // Preserve the status IDs used by the existing resignation/termination business rules.
        foreach (var legacyStatus in new[] { 125, 126, 127 })
        {
            if (!maps["lov"].TryGetValue(legacyStatus, out var oldTarget)) continue;
            await Execute("UPDATE e SET empstatusid=@canonical FROM dbo.EREmployeeinfohdr e JOIN dbo.ARG_LegacyMappings m ON m.Kind='EREmployeeinfohdr' AND m.TargetId=e.Id AND m.SourceKey=@source WHERE e.empstatusid=@old; UPDATE dbo.ARG_LegacyMappings SET TargetId=@canonical WHERE Kind='lov' AND LegacyId=@canonical AND SourceKey=@source",("@canonical",legacyStatus),("@source",sourceKey),("@old",oldTarget));
            maps["lov"][legacyStatus]=legacyStatus;
        }
        foreach (var row in employees)
        {
            var sourceClient = row["clientid"];
            var originalId=Int(row,"Id")!.Value;
            var wasImported=maps["EREmployeeinfohdr"].ContainsKey(originalId);
            var targetId = await Add("EREmployeeinfohdr",row,new()
            {
                ["clientid"]=1,["entityid"]=Mapped("OrgLegalentity",Int(row,"entityid")),
                ["depid"]=Mapped("OrgDepartment",Int(row,"depid")),["designationid"]=Mapped("OrgDesignation",Int(row,"designationid")),
                ["empstatusid"]=Mapped("lov",Int(row,"empstatusid")),["shiftid"]=Mapped("ETShifthdr",Int(row,"shiftid"))
            });
            if(wasImported) await Execute("UPDATE dbo.EREmployeeinfohdr SET resigndate=@date WHERE Id=@id",("@date",row["resigndate"]),("@id",targetId));
            await Execute("IF NOT EXISTS(SELECT 1 FROM dbo.ARG_ZkEmployeeLinks WHERE EmployeeId=@id) INSERT dbo.ARG_ZkEmployeeLinks(EmployeeId,SourceKey,SourceClientId,MachineCode) VALUES(@id,@source,@client,@code)", ("@id",targetId),("@source",sourceKey),("@client",sourceClient),("@code",row["machinecode"]));
        }
        foreach (var row in machines)
        {
            var sourceClient=row["clientid"];
            var targetId=await Add("SMMachineRegistration", row,new() { ["clientid"]=1,["entityid"]=Mapped("OrgLegalentity",Int(row,"entityid")) });
            await Execute("IF NOT EXISTS(SELECT 1 FROM dbo.ARG_ZkDeviceLinks WHERE MachineId=@id) INSERT dbo.ARG_ZkDeviceLinks(MachineId,SourceKey,SourceClientId) VALUES(@id,@source,@client)",("@id",targetId),("@source",sourceKey),("@client",sourceClient));
        }
        Console.WriteLine($"Attendance mappings ready: {employees.Count} employees; {machines.Count} devices. No passwords, source attendance, payroll or login accounts copied.");
    }
    private int? Mapped(string kind,int? id) => id.HasValue && maps[kind].TryGetValue(id.Value,out var targetId) ? targetId : null;
    private async Task<int> Add(string table,Dictionary<string,object?> row,Dictionary<string,object?> overrides)
    {
        int oldId=Int(row,"Id")!.Value;
        if (maps[table].TryGetValue(oldId,out var existing)) return existing;
        await using var max=Command($"SELECT ISNULL(MAX(id),0)+1 FROM dbo.[{table}]");
        int id=Convert.ToInt32(await max.ExecuteScalarAsync());
        var values=new Dictionary<string,object?>(row,StringComparer.OrdinalIgnoreCase) { ["Id"]=id };
        foreach (var pair in overrides) values[pair.Key]=pair.Value;
        var columns=values.Keys.ToArray();
        // All SQL identifiers above are fixed developer-owned column/table lists.
        await Execute($"INSERT dbo.[{table}] ({string.Join(',',columns.Select(c=>$"[{c}]"))}) VALUES ({string.Join(',',columns.Select((_,i)=>"@p"+i))})",columns.Select((c,i)=>("@p"+i,values[c])).ToArray());
        await Execute("INSERT dbo.ARG_LegacyMappings(SourceKey,Kind,LegacyId,TargetId) VALUES(@source,@kind,@old,@target)",("@source",sourceKey),("@kind",table),("@old",oldId),("@target",id));
        maps[table][oldId]=id;
        return id;
    }
    private async Task<List<Dictionary<string,object?>>> Read(string table,string columns,string filter="")
    {
        var rows=new List<Dictionary<string,object?>>();
        await using var command=new SqlCommand($"SELECT {columns} FROM dbo.[{table}] {filter}",source) { CommandTimeout=60 };
        await using var reader=await command.ExecuteReaderAsync();
        while(await reader.ReadAsync())
        {
            var row=new Dictionary<string,object?>(StringComparer.OrdinalIgnoreCase);
            for(int i=0;i<reader.FieldCount;i++) row[reader.GetName(i)]=reader.IsDBNull(i)?null:reader.GetValue(i);
            rows.Add(row);
        }
        return rows;
    }
    private static int? Int(Dictionary<string,object?> row,string key)=>row.TryGetValue(key,out var value)&&value!=null?Convert.ToInt32(value):null;
    private SqlCommand Command(string sql,params (string name,object? value)[] parameters)
    {
        var command=new SqlCommand(sql,target,transaction) { CommandTimeout=120 };
        foreach(var (name,value) in parameters) command.Parameters.AddWithValue(name,value??DBNull.Value);
        return command;
    }
    private async Task Execute(string sql,params (string name,object? value)[] parameters)
    {
        await using var command=Command(sql,parameters);await command.ExecuteNonQueryAsync();
    }
}
