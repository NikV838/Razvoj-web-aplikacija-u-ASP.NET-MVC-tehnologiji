using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Dtos;

public record RimDto(int Id, string Make, string Model, double SizeInJ, string Material);

public class RimUpsertDto
{
    [Required]
    [StringLength(80)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Model { get; set; } = string.Empty;

    [Range(1, 20)]
    public double SizeInJ { get; set; }

    [Required]
    [StringLength(80)]
    public string Material { get; set; } = string.Empty;
}
