using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.ViewModels;
using System.Security.Claims;

namespace SljemeTimeAttack.Controllers;

public class AccountController : Controller
{
    private static readonly string[] AllowedRegistrationRoles = ["Racer", "Spectator"];

    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly SljemeTimeAttackDbContext _context;
    private readonly ILogger<AccountController> _logger;
    private readonly bool _isGoogleLoginConfigured;
    private readonly bool _isFacebookLoginConfigured;

    public AccountController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        SljemeTimeAttackDbContext context,
        IConfiguration configuration,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _logger = logger;
        _isGoogleLoginConfigured =
            !string.IsNullOrWhiteSpace(configuration["Authentication:Google:ClientId"]) &&
            !string.IsNullOrWhiteSpace(configuration["Authentication:Google:ClientSecret"]);
        _isFacebookLoginConfigured =
            !string.IsNullOrWhiteSpace(configuration["Authentication:Facebook:AppId"]) &&
            !string.IsNullOrWhiteSpace(configuration["Authentication:Facebook:AppSecret"]);
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
            Register = new RegisterViewModel(),
            IsGoogleLoginConfigured = _isGoogleLoginConfigured,
            IsFacebookLoginConfigured = _isFacebookLoginConfigured
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
            _logger.LogInformation("User login succeeded. UserId: {UserId}, UserName: {UserName}", user?.Id, user?.UserName);
            TempData["WelcomeMessage"] = $"Welcome, {user?.UserName ?? viewModel.Email}";
            return LocalRedirect(GetSafeReturnUrl(viewModel.ReturnUrl));
        }

        _logger.LogWarning("User login failed for identifier {LoginIdentifier}", viewModel.Email);
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
            if (!await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

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
        _logger.LogInformation("User logout. UserName: {UserName}", User.Identity?.Name);
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        if (!IsExternalProviderConfigured(provider))
        {
            ModelState.AddModelError(string.Empty, $"{provider} login is not configured.");
            return AccountIndexWithLogin(new LoginViewModel { ReturnUrl = returnUrl });
        }

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
            var signedInUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            await EnsureUserRoleAndDriverProfileAsync(signedInUser);
            _logger.LogInformation("External user login succeeded. Provider: {Provider}, UserId: {UserId}, UserName: {UserName}", info.LoginProvider, signedInUser?.Id, signedInUser?.UserName);
            TempData["WelcomeMessage"] = $"Welcome, {signedInUser?.UserName ?? info.Principal.Identity?.Name ?? "driver"}";
            return LocalRedirect(GetSafeReturnUrl(returnUrl));
        }

        var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, "External provider did not return an email address.");
            return AccountIndexWithLogin(new LoginViewModel { ReturnUrl = returnUrl });
        }

        var displayName = info.Principal.FindFirst(ClaimTypes.Name)?.Value
            ?? info.Principal.Identity?.Name
            ?? email;
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            var linkResult = await _userManager.AddLoginAsync(existingUser, info);
            if (!linkResult.Succeeded && !linkResult.Errors.Any(error => error.Code == "LoginAlreadyAssociated"))
            {
                foreach (var error in linkResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return AccountIndexWithLogin(new LoginViewModel { ReturnUrl = returnUrl });
            }

            existingUser.EmailConfirmed = true;
            if (string.IsNullOrWhiteSpace(existingUser.DisplayName))
            {
                existingUser.DisplayName = displayName;
            }

            await _userManager.UpdateAsync(existingUser);
            await EnsureUserRoleAndDriverProfileAsync(existingUser);
            await _signInManager.SignInAsync(existingUser, isPersistent: false);
            _logger.LogInformation("External user login succeeded after account link. Provider: {Provider}, UserId: {UserId}, UserName: {UserName}", info.LoginProvider, existingUser.Id, existingUser.UserName);
            TempData["WelcomeMessage"] = $"Welcome, {existingUser.UserName}";
            return LocalRedirect(GetSafeReturnUrl(returnUrl));
        }

        var userName = await CreateUniqueUserNameFromEmailAsync(email);
        var user = new AppUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            DisplayName = displayName
        };

        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
            await _userManager.AddLoginAsync(user, info);
            await EnsureUserRoleAndDriverProfileAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("External user login succeeded after account creation. Provider: {Provider}, UserId: {UserId}, UserName: {UserName}", info.LoginProvider, user.Id, user.UserName);
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
            .Where(run => driver != null && (run.DriverId == driver.Id || (run.CarId.HasValue && carIds.Contains(run.CarId.Value))))
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
            Register = new RegisterViewModel(),
            IsGoogleLoginConfigured = _isGoogleLoginConfigured,
            IsFacebookLoginConfigured = _isFacebookLoginConfigured
        });

    private ViewResult AccountIndexWithRegister(RegisterViewModel register) =>
        View(nameof(Index), new AccountIndexViewModel
        {
            Login = new LoginViewModel { ReturnUrl = Url.Content("~/") },
            Register = register,
            IsGoogleLoginConfigured = _isGoogleLoginConfigured,
            IsFacebookLoginConfigured = _isFacebookLoginConfigured
        });

    private bool IsExternalProviderConfigured(string provider) =>
        provider switch
        {
            "Google" => _isGoogleLoginConfigured,
            "Facebook" => _isFacebookLoginConfigured,
            _ => false
        };

    private async Task EnsureUserRoleAndDriverProfileAsync(AppUser? user)
    {
        if (user == null)
        {
            return;
        }

        if (!await _userManager.IsInRoleAsync(user, "User"))
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        var hasDriverProfile = await _context.Drivers.AnyAsync(driver => driver.AppUserId == user.Id);
        if (hasDriverProfile)
        {
            return;
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
    }

    private async Task<string> CreateUniqueUserNameFromEmailAsync(string email)
    {
        var prefix = email.Split('@', 2)[0];
        var candidate = new string(prefix
            .Where(character => char.IsLetterOrDigit(character) || character is '_' or '-' or '.')
            .ToArray());

        if (string.IsNullOrWhiteSpace(candidate))
        {
            candidate = "driver";
        }

        var baseCandidate = candidate;
        var suffix = 1;
        while (await _userManager.FindByNameAsync(candidate) != null)
        {
            suffix++;
            candidate = $"{baseCandidate}{suffix}";
        }

        return candidate;
    }

    private static string GetRegisterErrorKey(string errorCode) =>
        errorCode switch
        {
            "DuplicateUserName" or "InvalidUserName" => "Register.Username",
            "DuplicateEmail" or "InvalidEmail" => "Register.Email",
            var code when code.StartsWith("Password", StringComparison.OrdinalIgnoreCase) => "Register.Password",
            _ => "Register.Username"
        };
}
