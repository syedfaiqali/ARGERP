using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Edxs.Tests;

// Probe the real MVC endpoint matcher without invoking databases or bypassing
// authentication in the application. This isolated host only reports route metadata.
public static class ControllerRouteProbe
{
    public static async Task AssertGetRoute(Assembly assembly, string path, string controller, string action)
    {
        var builder=WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddControllers().AddApplicationPart(assembly);
        builder.Services.AddAuthorization();
        await using var app=builder.Build();
        app.UseRouting();
        app.Use(async (context,next)=>
        {
            var endpoint=context.GetEndpoint();
            var descriptor=endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if(descriptor==null){await next(context);return;}
            context.Response.Headers["MatchedController"]=descriptor.ControllerName;
            context.Response.Headers["MatchedAction"]=descriptor.ActionName;
            context.Response.Headers["RequiresAuthorization"]=(endpoint!.Metadata.GetOrderedMetadata<IAuthorizeData>().Count>0).ToString();
            context.Response.StatusCode=StatusCodes.Status204NoContent;
        });
        app.UseAuthorization();
        app.MapControllers();
        await app.StartAsync();
        try
        {
            var address=app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
            using var client=new HttpClient {BaseAddress=new Uri(address)};
            using var response=await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
            Assert.Equal(controller,response.Headers.GetValues("MatchedController").Single());
            Assert.Equal(action,response.Headers.GetValues("MatchedAction").Single());
            Assert.Equal("True",response.Headers.GetValues("RequiresAuthorization").Single());
        }
        finally {await app.StopAsync();}
    }
}
