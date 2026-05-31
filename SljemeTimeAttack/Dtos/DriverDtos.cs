using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Dtos;

public record DriverDto(
    int Id,
    string Username,
    string Name,
    int Age,
    int YearsOfExperience,
    int? TeamId,
    TeamDto? Team,
    string? Email,
    string? PhoneNumber);

public class DriverUpsertDto
{
    [Required]
    [StringLength(40)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(16, 100)]
    public int Age { get; set; }

    [Range(0, 80)]
    public int YearsOfExperience { get; set; }

    public int? TeamId { get; set; }

    [EmailAddress]
    [StringLength(120)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? PhoneNumber { get; set; }
}
