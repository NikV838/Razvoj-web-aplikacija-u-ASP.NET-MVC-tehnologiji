using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Services;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IGarageDeletionService _garageDeletionService;

    public AdminController(UserManager<AppUser> userManager, IGarageDeletionService garageDeletionService)
    {
        _userManager = userManager;
        _garageDeletionService = garageDeletionService;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.UserName)
            .ToListAsync();

        var viewModels = new List<AdminUserViewModel>();
        foreach (var user in users)
        {
            viewModels.Add(new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            });
        }

        return View(viewModels);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["AdminUsersMessage"] = "User was not found.";
            return RedirectToAction(nameof(Users));
        }

        var currentUserId = _userManager.GetUserId(User);
        if (user.Id == currentUserId)
        {
            TempData["AdminUsersMessage"] = "You cannot delete the currently signed-in admin account.";
            return RedirectToAction(nameof(Users));
        }

        var result = await _garageDeletionService.DeleteAppUserAsync(user, User.Identity?.Name);
        TempData["AdminUsersMessage"] = result.Succeeded
            ? $"Deleted user {user.UserName}."
            : string.Join(" ", result.Errors.Select(error => error.Description));

        return RedirectToAction(nameof(Users));
    }
}
