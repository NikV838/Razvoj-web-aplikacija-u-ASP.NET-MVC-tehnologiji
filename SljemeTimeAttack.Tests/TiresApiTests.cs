using System.Net;
using System.Net.Http.Json;
using SljemeTimeAttack.Dtos;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class TiresApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public TiresApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        TestDatabase.Reset(_factory);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _factory.CreateClient().GetAsync("/api/tires");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByExistingId_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var tire = await ApiTestHelpers.CreateTireAsync(client);
        var response = await client.GetAsync($"/api/tires/{tire.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByMissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/tires/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidTire_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var rim = await ApiTestHelpers.CreateRimAsync(client);
        var response = await client.PostAsJsonAsync("/api/tires", ValidDto(rim.Id, "post"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidTire_ReturnsBadRequest()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/tires", new TireUpsertDto { Brand = "", Model = "", Type = "", SizeInMm = 1, Dot = "", RimId = 0 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidTire_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var tire = await ApiTestHelpers.CreateTireAsync(client);
        var rim = await ApiTestHelpers.CreateRimAsync(client);
        var response = await client.PutAsJsonAsync($"/api/tires/{tire.Id}", ValidDto(rim.Id, "put"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var rim = await ApiTestHelpers.CreateRimAsync(client);
        var response = await client.PutAsJsonAsync("/api/tires/999999", ValidDto(rim.Id, "missing"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingTire_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var tire = await ApiTestHelpers.CreateTireAsync(client);
        var response = await client.DeleteAsync($"/api/tires/{tire.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().DeleteAsync("/api/tires/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TireUpsertDto ValidDto(int rimId, string suffix) => new()
    {
        Brand = $"Brand {suffix}",
        Model = $"Model {suffix}",
        Type = "Semi-Slick",
        SizeInMm = 225,
        Dot = "2026",
        RimId = rimId
    };
}
