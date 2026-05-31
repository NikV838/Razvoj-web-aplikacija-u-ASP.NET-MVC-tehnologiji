namespace SljemeTimeAttack.ViewModels;

public class AccountIndexViewModel
{
    public LoginViewModel Login { get; set; } = new();

    public RegisterViewModel Register { get; set; } = new();
}
