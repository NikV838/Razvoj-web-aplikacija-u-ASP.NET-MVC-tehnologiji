using System.Net;
using System.Net.Http.Json;
using SljemeTimeAttack.Dtos;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class SuspensionsApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public SuspensionsApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        TestDatabase.Reset(_factory);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _factory.CreateClient().GetAsync("/api/suspensions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByExistingId_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var suspension = await ApiTestHelpers.CreateSuspensionAsync(client);
        var response = await client.GetAsync($"/api/suspensions/{suspension.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByMissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/suspensions/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidSuspension_ReturnsCreated()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/suspensions", ValidDto("post"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidSuspension_ReturnsBadRequest()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/suspensions", new SuspensionUpsertDto { Brand = "", Type = "", RideHeightMm = 1 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidSuspension_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var suspension = await ApiTestHelpers.CreateSuspensionAsync(client);
        var response = await client.PutAsJsonAsync($"/api/suspensions/{suspension.Id}", ValidDto("put"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().PutAsJsonAsync("/api/suspensions/999999", ValidDto("missing"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingSuspension_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var suspension = await ApiTestHelpers.CreateSuspensionAsync(client);
        var response = await client.DeleteAsync($"/api/suspensions/{suspension.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().DeleteAsync("/api/suspensions/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static SuspensionUpsertDto ValidDto(string suffix) => new()
    {
        Brand = $"Brand {suffix}",
        Type = "Coilover",
        HasFrontStrutBar = true,
        HasRearStrutBar = true,
        RideHeightMm = 95,
        IsHeightAdjustable = true,
        IsStiffnessAdjustable = true,
        FrontStiffness = 8,
        RearStiffness = 7
    };
}
