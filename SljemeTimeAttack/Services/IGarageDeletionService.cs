using Microsoft.AspNetCore.Identity;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Services;

public interface IGarageDeletionService
{
    Task DeleteCarAsync(Car car, string? userName = null, CancellationToken cancellationToken = default);

    Task DeleteDriverAsync(Driver driver, string? userName = null, CancellationToken cancellationToken = default);

    Task<IdentityResult> DeleteAppUserAsync(AppUser user, string? adminUserName = null, CancellationToken cancellationToken = default);
}
