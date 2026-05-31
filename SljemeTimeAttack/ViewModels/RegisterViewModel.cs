using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels;

public class RegisterViewModel
{
    [Required]
    [StringLength(40)]
    public string Username { get; set; } = string.Empty;

    [StringLength(80)]
    public string? DisplayName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Country { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
