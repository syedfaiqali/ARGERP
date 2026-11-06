using HumanResource.Service.Controllers;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HumanResource.Service.Tests;

public sealed class EmployeeReportingToLookupTests
{
    private static HumanResourceDbContext Db() => new(new DbContextOptionsBuilder<HumanResourceDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static EmployeeProfileV3Controller Controller(HumanResourceDbContext db, int? entity = 10)
    {
        var http = new DefaultHttpContext();
        http.Request.Headers["X-Client-Id"] = "1";
        if (entity.HasValue) http.Request.Headers["X-Entity-Id"] = entity.Value.ToString();
        return new(db, NullLogger<EmployeeProfileV3Controller>.Instance, new ConfigurationBuilder().Build(),
            new VacancyFulfillmentService(db, NullLogger<VacancyFulfillmentService>.Instance))
        { ControllerContext = new ControllerContext { HttpContext = http } };
    }

    private static EmployeeProfileLegacy Employee(int id, int? designation = null, int? reportingTo = null,
        int client = 1, int entity = 10, bool blocked = false) => new()
        { Id = id, ClientId = client, EntityId = entity, EmployeeCode = $"E{id}", FirstName = $"Person {id:D2}",
            DesignationId = designation, ReportingToEmployeeId = reportingTo, UtBlock = blocked };

    private static LookupItemDto[] Rows(ActionResult<IEnumerable<LookupItemDto>> response) =>
        Assert.IsAssignableFrom<IEnumerable<LookupItemDto>>(Assert.IsType<OkObjectResult>(response.Result).Value).ToArray();

    [Theory]
    [InlineData("CEO")]
    [InlineData("C.E.O.")]
    [InlineData("Chief Executive Officer")]
    [InlineData("Manager")]
    [InlineData("HR Manager")]
    [InlineData("Assistant Manager - Finance")]
    [InlineData("General Manager")]
    public async Task LeadershipIsIncludedWithoutExistingDirectReports(string designation)
    {
        await using var db = Db();
        db.Designations.Add(new OrgDesignationLegacy { Id = 1, ClientId = 1, Stxt = designation });
        db.EmployeeProfiles.AddRange(Employee(1, 1), Employee(2));
        await db.SaveChangesAsync();
        Assert.Equal(new[] { 1 }, Rows(await Controller(db).GetReportingToLookup()).Select(x => x.Id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    public async Task OnlyLeadershipAndSupervisorsInScopeAreOffered(int? exclude)
    {
        await using var db = Db();
        db.Designations.AddRange(
            new OrgDesignationLegacy { Id = 1, ClientId = 1, Stxt = "CEO" },
            new OrgDesignationLegacy { Id = 2, ClientId = 1, Stxt = "Management Trainee" },
            new OrgDesignationLegacy { Id = 3, ClientId = 2, Stxt = "Manager" },
            new OrgDesignationLegacy { Id = 4, ClientId = 1, Stxt = "Manager", UtBlock = true },
            new OrgDesignationLegacy { Id = 5, ClientId = 1, Code = "MGR", Stxt = "Department Head" });
        db.EmployeeProfiles.AddRange(
            Employee(1, 1), Employee(2, 2), Employee(3), Employee(4, reportingTo: 3),
            Employee(5, 1, blocked: true), Employee(6, 1, client: 2), Employee(7, 1, entity: 20),
            Employee(8, 3), Employee(9, 4), Employee(10, 5),
            Employee(11), Employee(12, reportingTo: 11, blocked: true),
            Employee(13), Employee(14, reportingTo: 13, client: 2),
            Employee(15), Employee(16, reportingTo: 15, entity: 20), Employee(17, reportingTo: 17));
        await db.SaveChangesAsync();
        var rows = Rows(await Controller(db).GetReportingToLookup(excludeEmployeeId: exclude));
        Assert.Equal(exclude.HasValue ? new[] { 3, 10 } : new[] { 1, 3, 10 }, rows.Select(x => x.Id));
    }

    [Fact]
    public async Task MissingEntityReturnsNoEmployees()
    {
        await using var db = Db();
        db.EmployeeProfiles.AddRange(Employee(1), Employee(2, reportingTo: 1));
        await db.SaveChangesAsync();
        Assert.Empty(Rows(await Controller(db, null).GetReportingToLookup()));
    }
}
