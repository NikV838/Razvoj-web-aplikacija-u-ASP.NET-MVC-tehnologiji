using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Services;

public class GarageDeletionService : IGarageDeletionService
{
    private readonly SljemeTimeAttackDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<GarageDeletionService> _logger;

    public GarageDeletionService(
        SljemeTimeAttackDbContext context,
        UserManager<AppUser> userManager,
        ILogger<GarageDeletionService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task DeleteCarAsync(Car car, string? userName = null, CancellationToken cancellationToken = default)
    {
        if (_context.Database.IsRelational())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            await DeleteCarCoreAsync(car, userName, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        await DeleteCarCoreAsync(car, userName, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteDriverAsync(Driver driver, string? userName = null, CancellationToken cancellationToken = default)
    {
        if (_context.Database.IsRelational())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            await DeleteDriverCoreAsync(driver, userName, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        await DeleteDriverCoreAsync(driver, userName, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IdentityResult> DeleteAppUserAsync(AppUser user, string? adminUserName = null, CancellationToken cancellationToken = default)
    {
        if (_context.Database.IsRelational())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var result = await DeleteAppUserCoreAsync(user, adminUserName, cancellationToken);
            if (result.Succeeded)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return result;
        }

        return await DeleteAppUserCoreAsync(user, adminUserName, cancellationToken);
    }

    private async Task<IdentityResult> DeleteAppUserCoreAsync(AppUser user, string? adminUserName, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers
            .Include(item => item.CarsOwned)
            .FirstOrDefaultAsync(item => item.AppUserId == user.Id, cancellationToken);

        if (driver != null)
        {
            await DeleteDriverCoreAsync(driver, adminUserName, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return result;
        }

        _logger.LogInformation("AppUser deleted. UserId: {UserId}, UserName: {UserName}, DeletedBy: {DeletedBy}", user.Id, user.UserName, adminUserName);
        return result;
    }

    private async Task DeleteDriverCoreAsync(Driver driver, string? userName, CancellationToken cancellationToken)
    {
        var cars = await _context.Cars
            .Where(car => car.DriverId == driver.Id)
            .ToListAsync(cancellationToken);

        var driverRuns = await _context.Runs
            .Where(run => run.DriverId == driver.Id)
            .ToListAsync(cancellationToken);

        foreach (var run in driverRuns)
        {
            SnapshotDriver(run, driver);
            run.DriverId = null;
        }

        foreach (var car in cars)
        {
            await DeleteCarCoreAsync(car, userName, cancellationToken);
        }

        _context.Drivers.Remove(driver);
        _logger.LogInformation("Driver deleted safely. DriverId: {DriverId}, DriverName: {DriverName}, User: {UserName}", driver.Id, driver.Name, userName);
    }

    private async Task DeleteCarCoreAsync(Car car, string? userName, CancellationToken cancellationToken)
    {
        var runs = await _context.Runs
            .Where(run => run.CarId == car.Id)
            .ToListAsync(cancellationToken);

        foreach (var run in runs)
        {
            SnapshotCar(run, car);
            run.CarId = null;
        }

        _context.Cars.Remove(car);
        _logger.LogInformation("Car deleted safely. CarId: {CarId}, Car: {Make} {Model}, User: {UserName}", car.Id, car.Make, car.Model, userName);
    }

    private static void SnapshotDriver(Run run, Driver driver)
    {
        if (string.IsNullOrWhiteSpace(run.DriverNameSnapshot))
        {
            run.DriverNameSnapshot = driver.Name;
        }
    }

    private static void SnapshotCar(Run run, Car car)
    {
        run.CarMakeSnapshot ??= car.Make;
        run.CarModelSnapshot ??= car.Model;
        run.CarRegistrationNumberSnapshot ??= car.RegistrationNumber;
        run.CarDisplayNameSnapshot ??= $"{car.Make} {car.Model}".Trim();
    }
}
