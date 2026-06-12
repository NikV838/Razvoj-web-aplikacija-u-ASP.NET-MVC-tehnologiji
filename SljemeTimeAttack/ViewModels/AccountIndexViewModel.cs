namespace SljemeTimeAttack.ViewModels;

public class AccountIndexViewModel
{
    public LoginViewModel Login { get; set; } = new();

    public RegisterViewModel Register { get; set; } = new();

    public bool IsGoogleLoginConfigured { get; set; }

    public bool IsFacebookLoginConfigured { get; set; }
}
