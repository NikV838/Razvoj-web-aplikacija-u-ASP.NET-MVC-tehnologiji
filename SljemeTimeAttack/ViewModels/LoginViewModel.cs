using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace SljemeTimeAttack.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public IEnumerable<AuthenticationScheme> ExternalLogins { get; set; } = [];
}
