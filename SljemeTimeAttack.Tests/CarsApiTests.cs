using System.Net;
using System.Net.Http.Json;
using SljemeTimeAttack.Dtos;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class CarsApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public CarsApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        TestDatabase.Reset(_factory);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _factory.CreateClient().GetAsync("/api/cars");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByExistingId_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var car = await ApiTestHelpers.CreateCarAsync(client);
        var response = await client.GetAsync($"/api/cars/{car.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByMissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/cars/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidCar_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var dto = await ValidDto(client, "post");
        var response = await client.PostAsJsonAsync("/api/cars", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidCar_ReturnsBadRequest()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/cars", new CarUpsertDto { Make = "", Model = "", Horsepower = 0, WeightKg = 1, Year = 1000, RegistrationNumber = "", TireId = 0, SuspensionId = 0 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidCar_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var car = await ApiTestHelpers.CreateCarAsync(client);
        var dto = await ValidDto(client, "put");
        var response = await client.PutAsJsonAsync($"/api/cars/{car.Id}", dto);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var dto = await ValidDto(client, "missing");
        var response = await client.PutAsJsonAsync("/api/cars/999999", dto);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingCar_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var car = await ApiTestHelpers.CreateCarAsync(client);
        var response = await client.DeleteAsync($"/api/cars/{car.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().DeleteAsync("/api/cars/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task<CarUpsertDto> ValidDto(HttpClient client, string suffix)
    {
        var driver = await ApiTestHelpers.CreateDriverAsync(client, suffix: suffix);
        var tire = await ApiTestHelpers.CreateTireAsync(client, suffix: suffix);
        var suspension = await ApiTestHelpers.CreateSuspensionAsync(client, suffix);
        return new CarUpsertDto
        {
            Make = $"Make {suffix}",
            Model = $"Model {suffix}",
            Horsepower = 240,
            WeightKg = 1190,
            Year = 2007,
            RegistrationNumber = $"ZG{Guid.NewGuid().ToString("N")[..6]}",
            DriverId = driver.Id,
            TireId = tire.Id,
            SuspensionId = suspension.Id
        };
    }
}
