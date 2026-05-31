using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Dtos;

public record TeamDto(int Id, string Name, string Country, string? Sponsor);

public class TeamUpsertDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Country { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Sponsor { get; set; }
}
