using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Enums;

namespace SljemeTimeAttack.Tests;

public static class ApiTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<TeamDto> CreateTeamAsync(HttpClient client, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var response = await client.PostAsJsonAsync("/api/teams", new TeamUpsertDto
        {
            Name = $"Team {suffix}",
            Country = "Croatia",
            Sponsor = "Test"
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TeamDto>(JsonOptions))!;
    }

    public static async Task<DriverDto> CreateDriverAsync(HttpClient client, int? teamId = null, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var response = await client.PostAsJsonAsync("/api/drivers", new DriverUpsertDto
        {
            Username = $"driver_{suffix}",
            Name = $"Driver {suffix}",
            Age = 28,
            YearsOfExperience = 6,
            TeamId = teamId,
            Email = $"driver-{suffix}@example.com",
            PhoneNumber = "12345678"
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DriverDto>(JsonOptions))!;
    }

    public static async Task<RimDto> CreateRimAsync(HttpClient client, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var response = await client.PostAsJsonAsync("/api/rims", new RimUpsertDto
        {
            Make = $"RimMake {suffix}",
            Model = $"RimModel {suffix}",
            SizeInJ = 8,
            Material = "Alloy"
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RimDto>(JsonOptions))!;
    }

    public static async Task<TireDto> CreateTireAsync(HttpClient client, int? rimId = null, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var rim = rimId.HasValue ? null : await CreateRimAsync(client, suffix);
        var response = await client.PostAsJsonAsync("/api/tires", new TireUpsertDto
        {
            Brand = $"TireBrand {suffix}",
            Model = $"TireModel {suffix}",
            Type = "Semi-Slick",
            SizeInMm = 225,
            Dot = "2026",
            RimId = rimId ?? rim!.Id
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TireDto>(JsonOptions))!;
    }

    public static async Task<SuspensionDto> CreateSuspensionAsync(HttpClient client, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var response = await client.PostAsJsonAsync("/api/suspensions", new SuspensionUpsertDto
        {
            Brand = $"SuspensionBrand {suffix}",
            Type = "Coilover",
            HasFrontStrutBar = true,
            HasRearStrutBar = true,
            RideHeightMm = 90,
            IsHeightAdjustable = true,
            IsStiffnessAdjustable = true,
            FrontStiffness = 8,
            RearStiffness = 7
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SuspensionDto>(JsonOptions))!;
    }

    public static async Task<CarDto> CreateCarAsync(HttpClient client, int? driverId = null, int? tireId = null, int? suspensionId = null, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var driver = driverId.HasValue ? null : await CreateDriverAsync(client, suffix: suffix);
        var tire = tireId.HasValue ? null : await CreateTireAsync(client, suffix: suffix);
        var suspension = suspensionId.HasValue ? null : await CreateSuspensionAsync(client, suffix);

        var response = await client.PostAsJsonAsync("/api/cars", new CarUpsertDto
        {
            Make = $"Make {suffix}",
            Model = $"Model {suffix}",
            Horsepower = 250,
            WeightKg = 1200,
            Year = 2006,
            RegistrationNumber = $"ZG{suffix[..6]}",
            DriverId = driverId ?? driver!.Id,
            TireId = tireId ?? tire!.Id,
            SuspensionId = suspensionId ?? suspension!.Id
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CarDto>(JsonOptions))!;
    }

    public static async Task<RunDto> CreateRunAsync(HttpClient client, int? driverId = null, int? carId = null, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var driver = driverId.HasValue ? null : await CreateDriverAsync(client, suffix: suffix);
        var car = carId.HasValue ? null : await CreateCarAsync(client, driverId ?? driver!.Id, suffix: suffix);

        var response = await client.PostAsJsonAsync("/api/runs", new RunUpsertDto
        {
            DriverId = driverId ?? driver!.Id,
            CarId = carId ?? car!.Id,
            Track = Track.Bliznec_Brestovac,
            BestTime = TimeSpan.FromSeconds(321),
            Date = new DateTime(2026, 7, 9, 12, 0, 0),
            Direction = DriveDirection.Uphill,
            Weather = WeatherCondition.Sunny
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RunDto>(JsonOptions))!;
    }

    public static async Task<RunNoteDto> CreateRunNoteAsync(HttpClient client, int? runId = null, string? suffix = null)
    {
        suffix ??= Guid.NewGuid().ToString("N")[..8];
        var run = runId.HasValue ? null : await CreateRunAsync(client, suffix: suffix);
        var response = await client.PostAsJsonAsync("/api/runnotes", new RunNoteUpsertDto
        {
            RunId = runId ?? run!.Id,
            Note = $"Note {suffix}",
            CreatedDate = new DateTime(2026, 7, 9, 13, 0, 0)
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RunNoteDto>(JsonOptions))!;
    }
}
