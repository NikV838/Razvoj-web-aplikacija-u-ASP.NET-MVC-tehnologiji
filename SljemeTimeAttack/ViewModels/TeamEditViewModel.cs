using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SljemeTimeAttack.ViewModels;

public class TeamEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Country { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Sponsor { get; set; }

    public string? ExistingImagePath { get; set; }

    public IFormFile? ImageFile { get; set; }
}
