using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Dtos;

public static class DtoMapper
{
    public static TeamDto ToDto(this Team team) =>
        new(team.Id, team.Name, team.Country, team.Sponsor, team.ImagePath);

    public static DriverDto ToDto(this Driver driver, bool includeTeam = true) =>
        new(
            driver.Id,
            driver.Username,
            driver.Name,
            driver.Age,
            driver.YearsOfExperience,
            driver.TeamId,
            includeTeam ? driver.Team?.ToDto() : null,
            driver.Email,
            driver.PhoneNumber);

    public static RimDto ToDto(this Rim rim) =>
        new(rim.Id, rim.Make, rim.Model, rim.SizeInJ, rim.Material);

    public static TireDto ToDto(this Tire tire, bool includeRim = true) =>
        new(tire.Id, tire.Brand, tire.Model, tire.Type, tire.SizeInMm, tire.Dot, tire.RimId, includeRim ? tire.Rim?.ToDto() : null);

    public static SuspensionDto ToDto(this Suspension suspension) =>
        new(
            suspension.Id,
            suspension.Type,
            suspension.Brand,
            suspension.HasFrontStrutBar,
            suspension.HasRearStrutBar,
            suspension.RideHeightMm,
            suspension.IsHeightAdjustable,
            suspension.IsStiffnessAdjustable,
            suspension.FrontStiffness,
            suspension.RearStiffness);

    public static CarDto ToDto(this Car car, bool includeRelated = true) =>
        new(
            car.Id,
            car.Make,
            car.Model,
            car.Horsepower,
            car.WeightKg,
            car.Year,
            car.RegistrationNumber,
            car.DriverId,
            includeRelated ? car.Driver?.ToDto() : null,
            car.TireId,
            includeRelated ? car.WheelSetup?.ToDto() : null,
            car.SuspensionId,
            includeRelated ? car.Suspension?.ToDto() : null);

    public static RunDto ToDto(this Run run, bool includeRelated = true) =>
        new(
            run.Id,
            run.DriverId,
            includeRelated ? run.Driver?.ToDto() : null,
            run.CarId,
            includeRelated ? run.Car?.ToDto() : null,
            run.Track,
            run.BestTime,
            run.Date,
            run.Direction,
            run.Weather,
            run.Files.Select(file => file.ToDto()).ToList());

    public static RunNoteDto ToDto(this RunNote note, bool includeRun = true) =>
        new(note.Id, note.RunId, includeRun ? note.Run?.ToDto() : null, note.Note, note.CreatedDate);

    public static RunFileDto ToDto(this RunFile file) =>
        new(file.Id, file.RunId, file.OriginalFileName, file.StoredFileName, file.ContentType, file.FilePath, file.UploadedAt);
}
