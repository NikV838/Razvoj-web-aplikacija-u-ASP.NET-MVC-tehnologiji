using Microsoft.AspNetCore.Identity;

namespace SljemeTimeAttack.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public string? Country { get; set; }

    public int? LinkedDriverId { get; set; }

    public Driver? LinkedDriver { get; set; }
}
