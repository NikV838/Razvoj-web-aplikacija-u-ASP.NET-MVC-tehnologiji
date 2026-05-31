using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new AccountIndexViewModel
        {
            Login = new LoginViewModel { ReturnUrl = GetSafeReturnUrl(returnUrl) },
            Register = new RegisterViewModel()
        });
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => RedirectToAction(nameof(Index), new { returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind(Prefix = "Login")] LoginViewModel viewModel)
    {
        viewModel.ExternalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
        if (!ModelState.IsValid) return AccountIndexWithLogin(viewModel);

        var user = await _userManager.FindByNameAsync(viewModel.Email)
            ?? await _userManager.FindByEmailAsync(viewModel.Email);
        var result = user == null
            ? Microsoft.AspNetCore.Identity.SignInResult.Failed
            : await _signInManager.PasswordSignInAsync(user.UserName!, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            TempData["WelcomeMessage"] = $"Welcome, {user?.UserName ?? viewModel.Email}";
            return LocalRedirect(GetSafeReturnUrl(viewModel.ReturnUrl));
        }

        ModelState.AddModelError("Login.Password", "Invalid username or password.");
        return AccountIndexWithLogin(viewModel);
    }

    [HttpGet]
    public IActionResult Register() => RedirectToAction(nameof(Index));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind(Prefix = "Register")] RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid) return AccountIndexWithRegister(viewModel);

        var user = new AppUser
        {
            UserName = viewModel.Username,
            Email = viewModel.Email,
            DisplayName = viewModel.DisplayName ?? viewModel.Username,
            Country = viewModel.Country
        };

        var result = await _userManager.CreateAsync(user, viewModel.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["WelcomeMessage"] = $"Welcome, {user.UserName}";
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(GetRegisterErrorKey(error.Code), error.Description);
        }

        return AccountIndexWithRegister(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, remoteError);
            return AccountIndexWithLogin(new LoginViewModel { ReturnUrl = returnUrl });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null) return RedirectToAction(nameof(Index), new { returnUrl });

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (signInResult.Succeeded)
        {
            TempData["WelcomeMessage"] = $"Welcome, {info.Principal.Identity?.Name ?? "driver"}";
            return LocalRedirect(GetSafeReturnUrl(returnUrl));
        }

        var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, "External provider did not return an email address.");
            return AccountIndexWithLogin(new LoginViewModel { ReturnUrl = returnUrl });
        }

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = info.Principal.Identity?.Name ?? email
        };

        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["WelcomeMessage"] = $"Welcome, {user.UserName}";
            return LocalRedirect(GetSafeReturnUrl(returnUrl));
        }

        foreach (var error in createResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return AccountIndexWithLogin(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    private string GetSafeReturnUrl(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/");

    private ViewResult AccountIndexWithLogin(LoginViewModel login) =>
        View(nameof(Index), new AccountIndexViewModel
        {
            Login = login,
            Register = new RegisterViewModel()
        });

    private ViewResult AccountIndexWithRegister(RegisterViewModel register) =>
        View(nameof(Index), new AccountIndexViewModel
        {
            Login = new LoginViewModel { ReturnUrl = Url.Content("~/") },
            Register = register
        });

    private static string GetRegisterErrorKey(string errorCode) =>
        errorCode switch
        {
            "DuplicateUserName" or "InvalidUserName" => "Register.Username",
            "DuplicateEmail" or "InvalidEmail" => "Register.Email",
            var code when code.StartsWith("Password", StringComparison.OrdinalIgnoreCase) => "Register.Password",
            _ => "Register.Username"
        };
}
