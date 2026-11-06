using System.Security.Cryptography;
using System.Text;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

internal static class ZkSyncIntegrationTest
{
    public static async Task RunAsync(string targetConnection)
    {
        var targetBuilder=new SqlConnectionStringBuilder(targetConnection);
        if(targetBuilder.InitialCatalog!="ARGERP_HR") throw new InvalidOperationException("Test only uses local ARG database.");
        var name="ARGERP_ZktSyncTest_"+Guid.NewGuid().ToString("N");
        var masterBuilder=new SqlConnectionStringBuilder(targetConnection) { InitialCatalog="master" };
        var sourceBuilder=new SqlConnectionStringBuilder(targetConnection) { InitialCatalog=name };
        var key=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sourceBuilder.DataSource.ToLowerInvariant()+"|"+name.ToLowerInvariant())));
        var linkKey="fixture-"+Guid.NewGuid().ToString("N");
        await using var master=new SqlConnection(masterBuilder.ConnectionString);await master.OpenAsync();
        await using var target=new SqlConnection(targetConnection);await target.OpenAsync();
        var machineId=0;
        async Task Exec(SqlConnection c,string sql,params (string,object)[] parameters)
        {
            await using var cmd=new SqlCommand(sql,c);foreach(var (k,v) in parameters)cmd.Parameters.AddWithValue(k,v);await cmd.ExecuteNonQueryAsync();
        }
        try
        {
            await Exec(master,$"CREATE DATABASE [{name}]");
            await using(var source=new SqlConnection(sourceBuilder.ConnectionString))
            {
                await source.OpenAsync();
                await Exec(source,"CREATE TABLE dbo.iclock_terminal(sn nvarchar(50),ip_address nvarchar(50)); CREATE TABLE dbo.iclock_transaction(Id int PRIMARY KEY,emp_code nvarchar(50),punch_time datetime2,punch_state int,terminal_sn nvarchar(50),sync_status int); INSERT dbo.iclock_terminal VALUES('FIXTURE','127.0.0.1'); INSERT dbo.iclock_transaction VALUES(1,'TEST1','2020-01-01T09:00:00',0,'FIXTURE',1),(2,'TEST1','2020-01-01T17:00:00',1,'FIXTURE',0),(3,'LATER','2020-01-02T09:00:00',0,'FIXTURE',0);");
            }
            await using(var cmd=new SqlCommand("SELECT ISNULL(MAX(Id),0)+1 FROM dbo.SMMachineRegistration",target))machineId=Convert.ToInt32(await cmd.ExecuteScalarAsync());
            await Exec(target,"INSERT dbo.SMMachineRegistration(Id,ltxt,clientid,entityid,inactive,utblock) VALUES(@id,'FIXTURE',1,1,0,0); INSERT dbo.ARG_ZkDeviceLinks(MachineId,SourceKey,SourceClientId) VALUES(@id,@key,501); INSERT dbo.ARG_ZkEmployeeLinks(EmployeeId,SourceKey,SourceClientId,MachineCode) SELECT Id,@key,501,'TEST1' FROM dbo.EREmployeeinfohdr WHERE employeecode='DEMO-1'; INSERT dbo.ARG_ZkEmployeeLinks(EmployeeId,SourceKey,SourceClientId,MachineCode) SELECT Id,@key,999,'TEST1' FROM dbo.EREmployeeinfohdr WHERE employeecode='DEMO-3';",("@id",machineId),("@key",linkKey));
            await using var db=new HumanResourceDbContext(new DbContextOptionsBuilder<HumanResourceDbContext>().UseSqlServer(targetConnection,sql=>sql.TranslateParameterizedCollectionsToConstants()).Options);
            var service=new ZkAttendanceSyncService(NullLogger<ZkAttendanceSyncService>.Instance,db,Options.Create(new ZkAttendanceSyncOptions { ConnectionString=sourceBuilder.ConnectionString }));
            var first=await service.SyncAttendanceAsync(CancellationToken.None);
            if(first.SyncedCount!=2)throw new Exception($"Expected 2 historical punches, received {first.SyncedCount} (skipped {first.SkippedCount}).");
            var second=await service.SyncAttendanceAsync(CancellationToken.None);
            if(second.SyncedCount!=0)throw new Exception("Second sync duplicated punches.");
            await Exec(target,"INSERT dbo.ARG_ZkEmployeeLinks(EmployeeId,SourceKey,SourceClientId,MachineCode) SELECT Id,@key,501,'LATER' FROM dbo.EREmployeeinfohdr WHERE employeecode='DEMO-2'",("@key",linkKey));
            var retry=await service.SyncAttendanceAsync(CancellationToken.None);
            if(retry.SyncedCount!=1)throw new Exception("Unmapped punch was not retried after mapping became available.");
            var wrongEmployee=await db.Employees.Where(e=>e.EmployeeCode=="DEMO-3").Select(e=>e.Id).SingleAsync();
            if(await db.EmployeeAttendances.AnyAsync(a=>a.MachineId==machineId&&a.EmpId==wrongEmployee))throw new Exception("Cross-client mapping assigned the wrong employee.");
            await using(var source=new SqlConnection(sourceBuilder.ConnectionString))
            {
                await source.OpenAsync();await using var command=new SqlCommand("SELECT SUM(sync_status) FROM dbo.iclock_transaction",source);
                if(Convert.ToInt32(await command.ExecuteScalarAsync())!=1)throw new Exception("Source sync flags were modified.");
            }
            Console.WriteLine("PASS ZKT: full history (including sync_status=1), repeat idempotency, deferred mapping retry, client isolation and unchanged source flags.");
        }
        finally
        {
            await Exec(target,"DELETE a FROM dbo.HRAtnintout a JOIN dbo.ARG_ZkImportReceipts r ON r.AttendanceId=a.id WHERE r.SourceKey=@source; DELETE dbo.ARG_ZkImportReceipts WHERE SourceKey=@source; DELETE dbo.ARG_ZkEmployeeLinks WHERE SourceKey=@link; DELETE dbo.ARG_ZkDeviceLinks WHERE SourceKey=@link; DELETE dbo.SMMachineRegistration WHERE Id=@machine AND ltxt='FIXTURE';",("@source",key),("@link",linkKey),("@machine",machineId));
            SqlConnection.ClearAllPools();
            if(!name.StartsWith("ARGERP_ZktSyncTest_",StringComparison.Ordinal)||name.Length!=51)throw new InvalidOperationException("Invalid fixture database name.");
            await Exec(master,$"IF DB_ID(N'{name}') IS NOT NULL BEGIN ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]; END");
        }
    }
}
