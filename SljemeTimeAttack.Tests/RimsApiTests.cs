using System.Net;
using System.Net.Http.Json;
using SljemeTimeAttack.Dtos;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class RimsApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public RimsApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        TestDatabase.Reset(_factory);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _factory.CreateClient().GetAsync("/api/rims");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByExistingId_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var rim = await ApiTestHelpers.CreateRimAsync(client);
        var response = await client.GetAsync($"/api/rims/{rim.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByMissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/rims/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidRim_ReturnsCreated()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/rims", new RimUpsertDto { Make = "Enkei", Model = "RPF1", SizeInJ = 8, Material = "Alloy" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidRim_ReturnsBadRequest()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/rims", new RimUpsertDto { Make = "", Model = "", SizeInJ = 0, Material = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidRim_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var rim = await ApiTestHelpers.CreateRimAsync(client);
        var response = await client.PutAsJsonAsync($"/api/rims/{rim.Id}", new RimUpsertDto { Make = "Updated", Model = "Wheel", SizeInJ = 9, Material = "Forged" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().PutAsJsonAsync("/api/rims/999999", new RimUpsertDto { Make = "Missing", Model = "Wheel", SizeInJ = 8, Material = "Alloy" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingRim_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var rim = await ApiTestHelpers.CreateRimAsync(client);
        var response = await client.DeleteAsync($"/api/rims/{rim.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().DeleteAsync("/api/rims/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
