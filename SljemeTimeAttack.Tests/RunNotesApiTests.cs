using System.Net;
using System.Net.Http.Json;
using SljemeTimeAttack.Dtos;
using Xunit;

namespace SljemeTimeAttack.Tests;

public class RunNotesApiTests : IClassFixture<SljemeApiFactory>
{
    private readonly SljemeApiFactory _factory;

    public RunNotesApiTests(SljemeApiFactory factory)
    {
        _factory = factory;
        TestDatabase.Reset(_factory);
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _factory.CreateClient().GetAsync("/api/runnotes");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByExistingId_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var note = await ApiTestHelpers.CreateRunNoteAsync(client);
        var response = await client.GetAsync($"/api/runnotes/{note.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByMissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().GetAsync("/api/runnotes/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidRunNote_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        var run = await ApiTestHelpers.CreateRunAsync(client);
        var response = await client.PostAsJsonAsync("/api/runnotes", ValidDto(run.Id, "post"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidRunNote_ReturnsBadRequest()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/runnotes", new RunNoteUpsertDto { RunId = 0, Note = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidRunNote_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var note = await ApiTestHelpers.CreateRunNoteAsync(client);
        var run = await ApiTestHelpers.CreateRunAsync(client);
        var response = await client.PutAsJsonAsync($"/api/runnotes/{note.Id}", ValidDto(run.Id, "put"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_MissingId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var run = await ApiTestHelpers.CreateRunAsync(client);
        var response = await client.PutAsJsonAsync("/api/runnotes/999999", ValidDto(run.Id, "missing"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingRunNote_ReturnsNoContent()
    {
        var client = _factory.CreateClient();
        var note = await ApiTestHelpers.CreateRunNoteAsync(client);
        var response = await client.DeleteAsync($"/api/runnotes/{note.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_MissingId_ReturnsNotFound()
    {
        var response = await _factory.CreateClient().DeleteAsync("/api/runnotes/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static RunNoteUpsertDto ValidDto(int runId, string suffix) => new()
    {
        RunId = runId,
        Note = $"Run note {suffix}",
        CreatedDate = new DateTime(2026, 7, 9, 14, 0, 0)
    };
}
