using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers;

public class AccountController : Controller
{
    private static readonly string[] AllowedRegistrationRoles = ["Racer", "Spectator"];

    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly SljemeTimeAttackDbContext _context;

    public AccountController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        SljemeTimeAttackDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
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
        if (!AllowedRegistrationRoles.Contains(viewModel.SelectedRole))
        {
            ModelState.AddModelError("Register.SelectedRole", "Choose Racer or Spectator.");
        }

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
            var driver = new Driver
            {
                Username = viewModel.Username,
                Name = viewModel.DisplayName ?? viewModel.Username,
                Age = 18,
                YearsOfExperience = 0,
                Email = viewModel.Email,
                AppUserId = user.Id
            };

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, viewModel.SelectedRole);
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
            await _userManager.AddToRoleAsync(user, "Spectator");
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

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var driver = await _context.Drivers
            .Include(item => item.Team)
            .Include(item => item.CarsOwned)
                .ThenInclude(car => car.WheelSetup)
            .Include(item => item.Runs)
                .ThenInclude(run => run.Car)
            .FirstOrDefaultAsync(item => item.AppUserId == user.Id);

        var carIds = driver?.CarsOwned.Select(car => car.Id).ToList() ?? [];
        var runs = await _context.Runs
            .Include(run => run.Car)
            .Include(run => run.Driver)
            .Where(run => driver != null && (run.DriverId == driver.Id || carIds.Contains(run.CarId)))
            .OrderByDescending(run => run.Date)
            .ToListAsync();

        return View(new MyGarageViewModel
        {
            UserName = user.UserName ?? string.Empty,
            Driver = driver,
            Cars = driver?.CarsOwned.ToList() ?? [],
            Runs = runs
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> CreateDriverProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var existingDriver = await _context.Drivers.FirstOrDefaultAsync(item => item.AppUserId == user.Id);
        if (existingDriver != null)
        {
            return RedirectToAction(nameof(Profile));
        }

        var driver = new Driver
        {
            Username = user.UserName ?? user.Email ?? "driver",
            Name = user.DisplayName ?? user.UserName ?? user.Email ?? "Driver",
            Age = 18,
            YearsOfExperience = 0,
            Email = user.Email,
            AppUserId = user.Id
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Profile));
    }

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
