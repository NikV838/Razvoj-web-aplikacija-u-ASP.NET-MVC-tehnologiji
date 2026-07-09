using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class DriversApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public DriversApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        TestDatabase.Reset(_factory);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _factory.CreateClient().GetAsync("/api/drivers");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByExistingId_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var driver = await ApiTestHelpers.CreateDriverAsync(client);
        var response = await client.GetAsync($"/api/drivers/{driver.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByMissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/drivers/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidDriver_ReturnsCreated()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/drivers", ValidDto("post"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidDriver_ReturnsBadRequest()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/drivers", new DriverUpsertDto { Username = "", Name = "", Age = 1, YearsOfExperience = -1 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidDriver_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var driver = await ApiTestHelpers.CreateDriverAsync(client);
        var response = await client.PutAsJsonAsync($"/api/drivers/{driver.Id}", ValidDto("put"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().PutAsJsonAsync("/api/drivers/999999", ValidDto("missing"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingDriver_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var driver = await ApiTestHelpers.CreateDriverAsync(client);
        var response = await client.DeleteAsync($"/api/drivers/{driver.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().DeleteAsync("/api/drivers/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task NormalUser_CannotEditAnotherUsersDriver()
    {
        var driverId = SeedDriver("other-user-id");
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-UserId", "normal-user-id");
        client.DefaultRequestHeaders.Add("X-Test-Roles", "User,Racer");

        var response = await client.PutAsJsonAsync($"/api/drivers/{driverId}", ValidDto("forbidden"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private int SeedDriver(string appUserId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SljemeTimeAttackDbContext>();
        var driver = new Driver
        {
            Username = $"seed_{Guid.NewGuid():N}",
            Name = "Seed Driver",
            Age = 31,
            YearsOfExperience = 8,
            AppUserId = appUserId
        };
        context.Drivers.Add(driver);
        context.SaveChanges();
        return driver.Id;
    }

    private static DriverUpsertDto ValidDto(string suffix) => new()
    {
        Username = $"driver_{suffix}_{Guid.NewGuid():N}"[..30],
        Name = $"Driver {suffix}",
        Age = 29,
        YearsOfExperience = 5,
        Email = $"driver-{Guid.NewGuid():N}@example.com",
        PhoneNumber = "12345678"
    };
}
