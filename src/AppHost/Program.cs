using Aspire.Hosting;
var builder = DistributedApplication.CreateBuilder(args);
var source = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, ".."));
builder.AddProject("hr", Path.Combine(source, "Services/HumanResource.Service/HumanResource.Service.csproj"))
    .WithHttpEndpoint(port: 5182, targetPort: 5182, isProxied: false).WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:5182");
builder.AddProject("organization", Path.Combine(source, "Services/Organization.Service/Organization.Service.csproj"))
    .WithHttpEndpoint(port: 5183, targetPort: 5183, isProxied: false).WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:5183");
builder.AddProject("gateway", Path.Combine(source, "Gateway/Yarp.Gateway/Yarp.Gateway.csproj"))
    .WithHttpEndpoint(port: 5181, targetPort: 5181, isProxied: false).WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:5181");
builder.AddProject("workflow", Path.Combine(source, "Services/Workflow.Service/Workflow.Service.csproj"))
    .WithHttpEndpoint(port: 5184, targetPort: 5184, isProxied: false).WithEnvironment("ASPNETCORE_URLS", "http://127.0.0.1:5184");
await builder.Build().RunAsync();
