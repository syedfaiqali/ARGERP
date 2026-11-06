using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Workflow.Service.Data;

public sealed class WorkflowDesignTimeFactory : IDesignTimeDbContextFactory<WorkflowDbContext>
{
    public WorkflowDbContext CreateDbContext(string[] args)
    {
        // Read the same OrganizationDB connection the runtime binds to, from the same
        // appsettings files, so scaffolding never targets a different database than the
        // service. Aspire injects ConnectionStrings__OrganizationDB at run time; at design
        // time dotnet ef runs this as a plain console app, so the files are the source.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var connection = configuration.GetConnectionString("OrganizationDB")
            ?? throw new InvalidOperationException("ConnectionStrings:OrganizationDB is required. Pass dotnet ef --connection to override.");
        return new(new DbContextOptionsBuilder<WorkflowDbContext>().UseSqlServer(connection).Options);
    }
}
