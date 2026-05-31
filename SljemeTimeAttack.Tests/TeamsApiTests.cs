using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class TeamsApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public TeamsApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SljemeTimeAttackDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/teams");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var teams = await response.Content.ReadFromJsonAsync<List<TeamDto>>();
        Assert.NotNull(teams);
        Assert.NotEmpty(teams);
    }

    [Fact]
    public async Task GetByInvalidId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/teams/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidTeam_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var dto = new TeamUpsertDto { Name = "Apex Works", Country = "Croatia", Sponsor = "Sljeme" };

        var response = await client.PostAsJsonAsync("/api/teams", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TeamDto>();
        Assert.NotNull(created);
        Assert.Equal(dto.Name, created.Name);
    }

    [Fact]
    public async Task Post_InvalidTeam_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var dto = new TeamUpsertDto { Name = "", Country = "", Sponsor = "Invalid" };

        var response = await client.PostAsJsonAsync("/api/teams", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidTeam_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/teams", new TeamUpsertDto { Name = "Update Me", Country = "Croatia" });
        var created = await createResponse.Content.ReadFromJsonAsync<TeamDto>();

        var response = await client.PutAsJsonAsync($"/api/teams/{created!.Id}", new TeamUpsertDto { Name = "Updated", Country = "Slovenia" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_InvalidId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/teams/999999", new TeamUpsertDto { Name = "Missing", Country = "Croatia" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ValidTeam_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/teams", new TeamUpsertDto { Name = "Delete Me", Country = "Croatia" });
        var created = await createResponse.Content.ReadFromJsonAsync<TeamDto>();

        var response = await client.DeleteAsync($"/api/teams/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_InvalidId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/teams/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
