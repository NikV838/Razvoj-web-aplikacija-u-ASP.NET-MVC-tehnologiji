using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Dtos;

public record TireDto(
    int Id,
    string Brand,
    string Model,
    string Type,
    double SizeInMm,
    string Dot,
    int RimId,
    RimDto? Rim);

public class TireUpsertDto
{
    [Required]
    [StringLength(80)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Type { get; set; } = string.Empty;

    [Range(100, 400)]
    public double SizeInMm { get; set; }

    [Required]
    [StringLength(20)]
    public string Dot { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RimId { get; set; }
}
