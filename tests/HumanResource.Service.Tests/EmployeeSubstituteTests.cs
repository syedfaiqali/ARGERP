using System.Text.Json;
using HumanResource.Service.Controllers;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HumanResource.Service.Tests;

public sealed class EmployeeSubstituteTests
{
    private static HumanResourceDbContext Db() => new(new DbContextOptionsBuilder<HumanResourceDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);

    private static EmployeeProfileV3Controller Controller(HumanResourceDbContext db)
    {
        var http = new DefaultHttpContext();
        http.Request.Headers["X-Client-Id"] = "1";
        http.Request.Headers["X-Entity-Id"] = "10";
        return new(db, NullLogger<EmployeeProfileV3Controller>.Instance, new ConfigurationBuilder().Build(),
            new VacancyFulfillmentService(db, NullLogger<VacancyFulfillmentService>.Instance))
        { ControllerContext = new ControllerContext { HttpContext = http } };
    }

    private static T Request<T>(int? substitute) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(new
    {
        Id = 1, EntityId = 10, EmployeeCode = "NEW", FirstName = "Test Employee",
        PositionTypeId = 1, JoiningDate = DateTime.Today, EmployeeTypeId = 1, EmployeeCategoryId = 1,
        DepartmentId = 1, DesignationId = 1, StatusId = 125, GradeId = 1,
        DateOfBirth = DateTime.Today.AddYears(-25), GenderId = 1, BirthCountryId = 1, BirthCityId = 1,
        NationalityId = 1, CellNo = "03001234567", ShiftId = 1, SubstituteEmployeeId = substitute
    }))!;

    private static async Task Seed(HumanResourceDbContext db)
    {
        db.OrgLegalentities.Add(new OrgLegalentityLegacy { Id = 10, ClientId = 1 });
        db.EmployeeGrades.Add(new EmployeeGradeLegacy { Id = 1, ClientId = 1, EntityId = 10, Code = "G1" });
        db.Designations.Add(new OrgDesignationLegacy { Id = 1, ClientId = 1, Stxt = "CEO" });
        db.EmployeeProfiles.AddRange(
            new EmployeeProfileLegacy { Id = 1, ClientId = 1, EntityId = 10, EmployeeCode = "E1" },
            new EmployeeProfileLegacy { Id = 2, ClientId = 1, EntityId = 10, DesignationId = 1 },
            new EmployeeProfileLegacy { Id = 3, ClientId = 1, EntityId = 10 },
            new EmployeeProfileLegacy { Id = 4, ClientId = 1, EntityId = 20, DesignationId = 1 },
            new EmployeeProfileLegacy { Id = 5, ClientId = 1, EntityId = 10, DesignationId = 1, UtBlock = true },
            new EmployeeProfileLegacy { Id = 6, ClientId = 2, EntityId = 10, DesignationId = 1 });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task SubstituteCanBeCreatedReadEditedAndCleared()
    {
        await using var db = Db();
        await Seed(db);
        var controller = Controller(db);
        var result = await controller.Create(Request<CreateEmployeeProfileV3Request>(2));
        var created = Assert.IsType<EmployeeProfileV3Dto>(Assert.IsType<CreatedAtActionResult>(result.Result).Value);
        Assert.Equal(2, created.SubstituteEmployeeId);
        db.ChangeTracker.Clear();
        var read = await controller.GetById(created.Id);
        Assert.Equal(2, Assert.IsType<EmployeeProfileV3Dto>(Assert.IsType<OkObjectResult>(read.Result).Value).SubstituteEmployeeId);

        var update = Request<UpdateEmployeeProfileV3Request>(2) with { Id = created.Id, EmployeeCode = created.EmployeeCode };
        Assert.IsType<NoContentResult>(await controller.Update(update));
        Assert.IsType<NoContentResult>(await controller.Update(update with { SubstituteEmployeeId = null }));
        db.ChangeTracker.Clear();
        Assert.Null((await db.EmployeeProfiles.FindAsync(created.Id))!.SubstituteEmployeeId);
    }

    [Fact]
    public void EmployeeQueriesIncludeSubstituteColumn()
    {
        using var db = new HumanResourceDbContext(new DbContextOptionsBuilder<HumanResourceDbContext>()
            .UseSqlServer("Server=localhost;Database=ModelOnly;Trusted_Connection=True;TrustServerCertificate=True")
            .Options);

        Assert.NotNull(db.Model.FindEntityType(typeof(EmployeeProfileLegacy))!
            .FindProperty(nameof(EmployeeProfileLegacy.SubstituteEmployeeId)));
        Assert.Contains("substituteemployeeid", db.EmployeeProfiles.ToQueryString(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(999)]
    public async Task SubstituteMustBeAnotherManagerFromSameEntityAndTenant(int substitute)
    {
        await using var db = Db();
        await Seed(db);
        var response = await Controller(db).Update(Request<UpdateEmployeeProfileV3Request>(substitute));
        Assert.Contains("Substitute", Assert.IsType<string>(Assert.IsType<BadRequestObjectResult>(response).Value));
        Assert.Null((await db.EmployeeProfiles.FindAsync(1))!.SubstituteEmployeeId);
    }
}
