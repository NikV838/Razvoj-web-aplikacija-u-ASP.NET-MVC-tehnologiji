using System.Net;
using System.Net.Http.Json;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Enums;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class RunsApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public RunsApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        TestDatabase.Reset(_factory);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _factory.CreateClient().GetAsync("/api/runs");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByExistingId_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var run = await ApiTestHelpers.CreateRunAsync(client);
        var response = await client.GetAsync($"/api/runs/{run.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByMissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/runs/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidRun_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var dto = await ValidDto(client);
        var response = await client.PostAsJsonAsync("/api/runs", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidRun_ReturnsBadRequest()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/runs", new RunUpsertDto { DriverId = 0, CarId = 0, BestTime = null, Date = null });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidRun_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var run = await ApiTestHelpers.CreateRunAsync(client);
        var dto = await ValidDto(client);
        var response = await client.PutAsJsonAsync($"/api/runs/{run.Id}", dto);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var dto = await ValidDto(client);
        var response = await client.PutAsJsonAsync("/api/runs/999999", dto);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingRun_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var run = await ApiTestHelpers.CreateRunAsync(client);
        var response = await client.DeleteAsync($"/api/runs/{run.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().DeleteAsync("/api/runs/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RunFile_UploadListDelete_Works()
    {
        var client = _factory.CreateClient();
        var run = await ApiTestHelpers.CreateRunAsync(client);
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent("hello sljeme"u8.ToArray()), "file", "note.txt");

        var uploadResponse = await client.PostAsync($"/api/runs/{run.Id}/files", content);

        Assert.Equal(HttpStatusCode.Created, uploadResponse.StatusCode);
        var uploaded = await uploadResponse.Content.ReadFromJsonAsync<RunFileDto>();
        Assert.NotNull(uploaded);

        var listResponse = await client.GetAsync($"/api/runs/{run.Id}/files");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var files = await listResponse.Content.ReadFromJsonAsync<List<RunFileDto>>();
        Assert.Contains(files!, file => file.Id == uploaded!.Id);

        var deleteResponse = await client.DeleteAsync($"/api/runs/files/{uploaded!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    private static async Task<RunUpsertDto> ValidDto(HttpClient client)
    {
        var driver = await ApiTestHelpers.CreateDriverAsync(client);
        var car = await ApiTestHelpers.CreateCarAsync(client, driver.Id);
        return new RunUpsertDto
        {
            DriverId = driver.Id,
            CarId = car.Id,
            Track = Track.Bliznec_Brestovac,
            BestTime = TimeSpan.FromSeconds(305),
            Date = new DateTime(2026, 7, 9, 12, 30, 0),
            Direction = DriveDirection.Downhill,
            Weather = WeatherCondition.Cloudy
        };
    }
}
