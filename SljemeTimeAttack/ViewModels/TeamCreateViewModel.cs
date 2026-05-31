using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels;

public class TeamCreateViewModel
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
