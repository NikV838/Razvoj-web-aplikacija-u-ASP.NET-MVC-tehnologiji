using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Dtos;

public record CarDto(
    int Id,
    string Make,
    string Model,
    int Horsepower,
    double WeightKg,
    int Year,
    string RegistrationNumber,
    int? DriverId,
    DriverDto? Driver,
    int TireId,
    TireDto? WheelSetup,
    int SuspensionId,
    SuspensionDto? Suspension);

public class CarUpsertDto
{
    [Required]
    [StringLength(80)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Range(1, 3000)]
    public int Horsepower { get; set; }

    [Range(100, 5000)]
    public double WeightKg { get; set; }

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [StringLength(30)]
    public string RegistrationNumber { get; set; } = string.Empty;

    public int? DriverId { get; set; }

    [Range(1, int.MaxValue)]
    public int TireId { get; set; }

    [Range(1, int.MaxValue)]
    public int SuspensionId { get; set; }
}
