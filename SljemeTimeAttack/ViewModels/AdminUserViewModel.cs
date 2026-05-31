namespace SljemeTimeAttack.ViewModels;

public class AdminUserViewModel
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public IReadOnlyCollection<string> Roles { get; set; } = [];
}
