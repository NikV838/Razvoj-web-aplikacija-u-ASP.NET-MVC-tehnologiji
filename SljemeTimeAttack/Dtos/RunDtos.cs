using System.ComponentModel.DataAnnotations;
using SljemeTimeAttack.Enums;

namespace SljemeTimeAttack.Dtos;

public record RunFileDto(
    int Id,
    int RunId,
    string OriginalFileName,
    string StoredFileName,
    string ContentType,
    string FilePath,
    DateTime UploadedAt);

public record RunDto(
    int Id,
    int DriverId,
    DriverDto? Driver,
    int CarId,
    CarDto? Car,
    Track Track,
    TimeSpan BestTime,
    DateTime Date,
    DriveDirection Direction,
    WeatherCondition Weather,
    IReadOnlyCollection<RunFileDto> Files);

public class RunUpsertDto
{
    [Range(1, int.MaxValue)]
    public int DriverId { get; set; }

    [Range(1, int.MaxValue)]
    public int CarId { get; set; }

    public Track Track { get; set; }

    [Required]
    public TimeSpan? BestTime { get; set; }

    [Required]
    public DateTime? Date { get; set; }

    public DriveDirection Direction { get; set; }

    public WeatherCondition Weather { get; set; }
}
