using Aspire.Hosting;
using Microsoft.Extensions.Configuration;
var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional:false).AddEnvironmentVariables();
var source = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, ".."));
// External SQL databases: AppHost injects the approved connection strings; it never renames SM databases.
var hrDatabase = builder.AddConnectionString("OrganizationDB");
var authDatabase = builder.AddConnectionString("UserManagementDB");
// Dedicated ARG containers/volumes keep the existing EDXS infrastructure independent.
var redis = builder.AddRedis("redis").WithImageTag("7.4").WithContainerName("argerp-redis").WithLifetime(Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent);
var rabbitUser = builder.AddParameter("arg-rabbit-user");
var rabbitPassword = builder.AddParameter("arg-rabbit-password",secret:true);
var rabbitMq = builder.AddRabbitMQ("rabbitmq",userName:rabbitUser,password:rabbitPassword).WithImageTag("4.0-management").WithManagementPlugin()
    .WithContainerName("argerp-rabbitmq").WithEnvironment("RABBITMQ_NODENAME","rabbit@localhost")
    .WithDataVolume("argerp-rabbitmq-data").WithLifetime(Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent);
var hr = builder.AddProject("hr", Path.Combine(source, "Services/HumanResource.Service/HumanResource.Service.csproj"), launchProfileName:null)
    .WithHttpEndpoint(port:5182,targetPort:5182,isProxied:false).WithEnvironment("ASPNETCORE_URLS","http://127.0.0.1:5182")
    .WithReference(hrDatabase).WithReference(authDatabase).WithReference(redis).WithReference(rabbitMq).WaitFor(redis).WaitFor(rabbitMq);
var organization = builder.AddProject("organization", Path.Combine(source, "Services/Organization.Service/Organization.Service.csproj"), launchProfileName:null)
    .WithHttpEndpoint(port:5183,targetPort:5183,isProxied:false).WithEnvironment("ASPNETCORE_URLS","http://127.0.0.1:5183")
    .WithReference(hrDatabase).WithReference(authDatabase).WithReference(redis).WithReference(rabbitMq).WaitFor(redis).WaitFor(rabbitMq);
builder.AddProject("gateway", Path.Combine(source, "Gateway/Yarp.Gateway/Yarp.Gateway.csproj"), launchProfileName:null)
    .WithHttpEndpoint(port:5181,targetPort:5181,isProxied:false).WithEnvironment("ASPNETCORE_URLS","http://127.0.0.1:5181")
    .WaitFor(hr).WaitFor(organization);
await builder.Build().RunAsync();
