using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Handlers.Commands;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.Service.Tests;

public sealed class HolidayTypeCommandTests
{
    private static HumanResourceDbContext CreateContext() => new(
        new DbContextOptionsBuilder<HumanResourceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static ETHolidaytypeLegacy Existing(int id, string code, int clientId = 1, int entityId = 10) => new()
    {
        Id = id, Code = code, Stxt = $"Holiday {id}", ClientId = clientId, EntityId = entityId,
    };

    private static CreateHolidayTypeV3Command Create(string code = "", int entityId = 10) =>
        new(new CreateHolidayTypeV3Request(code, "New Holiday", null, "#123456", true), 1, entityId);

    [Fact]
    public async Task Empty_code_is_generated_and_skips_existing_manual_code()
    {
        await using var db = CreateContext();
        db.HolidayTypes.Add(Existing(1, "0002"));
        await db.SaveChangesAsync();
        var result = await new HolidayTypeCommandHandlers(db).Handle(Create(), default);
        Assert.Equal(2, result.Id);
        Assert.Equal("0003", result.Code);
        Assert.Equal("0003", (await db.HolidayTypes.FindAsync(2))!.Code);
    }

    [Fact]
    public async Task Duplicate_code_is_rejected_on_create_after_trimming_and_case_normalization()
    {
        await using var db = CreateContext();
        db.HolidayTypes.Add(Existing(1, " ABC "));
        await db.SaveChangesAsync();
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new HolidayTypeCommandHandlers(db).Handle(Create(" abc "), default));
        Assert.Contains("code 'abc' already exists", error.Message);
        Assert.Equal(1, await db.HolidayTypes.CountAsync());
    }

    [Fact]
    public async Task Update_rejects_another_records_code_but_accepts_its_own_code()
    {
        await using var db = CreateContext();
        db.HolidayTypes.AddRange(Existing(1, "0001"), Existing(2, "0002"));
        await db.SaveChangesAsync();
        var handler = new HolidayTypeCommandHandlers(db);
        var request = new UpdateHolidayTypeV3Request(2, "0001", "Updated Holiday", null, "#123456", true);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new UpdateHolidayTypeV3Command(request, 1, 10), default));
        Assert.Equal("0002", (await db.HolidayTypes.FindAsync(2))!.Code);
        Assert.True(await handler.Handle(new UpdateHolidayTypeV3Command(request with { Code = "0002" }, 1, 10), default));
    }

    [Fact]
    public async Task Code_validation_is_scoped_to_client_and_entity()
    {
        await using var db = CreateContext();
        db.HolidayTypes.AddRange(Existing(1, "0001", entityId: 11), Existing(2, "0001", clientId: 2));
        await db.SaveChangesAsync();
        var result = await new HolidayTypeCommandHandlers(db).Handle(Create("0001"), default);
        Assert.Equal("0001", result.Code);
    }
}
