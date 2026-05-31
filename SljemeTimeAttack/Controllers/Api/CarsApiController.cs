using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;

    public CarsController(SljemeTimeAttackDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarDto>>> GetAll([FromQuery] string? search)
    {
        var query = IncludeCarGraph(_context.Cars);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(car =>
                car.Make.ToLower().Contains(term) ||
                car.Model.ToLower().Contains(term) ||
                car.RegistrationNumber.ToLower().Contains(term) ||
                (car.Driver != null && car.Driver.Name.ToLower().Contains(term)) ||
                car.WheelSetup.Brand.ToLower().Contains(term) ||
                car.Suspension.Brand.ToLower().Contains(term));
        }

        return Ok((await query.OrderBy(car => car.Make).ThenBy(car => car.Model).ToListAsync()).Select(car => car.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarDto>> GetById(int id)
    {
        var car = await IncludeCarGraph(_context.Cars).FirstOrDefaultAsync(car => car.Id == id);
        return car == null ? NotFound() : Ok(car.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost]
    public async Task<ActionResult<CarDto>> Create(CarUpsertDto dto)
    {
        if (!await ApplyDriverOwnership(dto)) return Forbid();
        if (!await ReferencesExist(dto)) return ValidationProblem(ModelState);

        var car = new Car
        {
            Make = dto.Make,
            Model = dto.Model,
            Horsepower = dto.Horsepower,
            WeightKg = dto.WeightKg,
            Year = dto.Year,
            RegistrationNumber = dto.RegistrationNumber,
            DriverId = dto.DriverId,
            TireId = dto.TireId,
            SuspensionId = dto.SuspensionId
        };

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();
        car = await IncludeCarGraph(_context.Cars).FirstAsync(item => item.Id == car.Id);

        return CreatedAtAction(nameof(GetById), new { id = car.Id }, car.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CarUpsertDto dto)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        if (!await CanManageCar(car)) return Forbid();
        if (!await ApplyDriverOwnership(dto)) return Forbid();
        if (!await ReferencesExist(dto)) return ValidationProblem(ModelState);

        car.Make = dto.Make;
        car.Model = dto.Model;
        car.Horsepower = dto.Horsepower;
        car.WeightKg = dto.WeightKg;
        car.Year = dto.Year;
        car.RegistrationNumber = dto.RegistrationNumber;
        car.DriverId = dto.DriverId;
        car.TireId = dto.TireId;
        car.SuspensionId = dto.SuspensionId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        if (!await CanManageCar(car)) return Forbid();

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> ReferencesExist(CarUpsertDto dto)
    {
        var isValid = true;
        if (dto.DriverId.HasValue && !await _context.Drivers.AnyAsync(driver => driver.Id == dto.DriverId.Value))
        {
            ModelState.AddModelError(nameof(dto.DriverId), "Driver does not exist.");
            isValid = false;
        }

        if (!await _context.Tires.AnyAsync(tire => tire.Id == dto.TireId))
        {
            ModelState.AddModelError(nameof(dto.TireId), "Tire does not exist.");
            isValid = false;
        }

        if (!await _context.Suspensions.AnyAsync(suspension => suspension.Id == dto.SuspensionId))
        {
            ModelState.AddModelError(nameof(dto.SuspensionId), "Suspension does not exist.");
            isValid = false;
        }

        return isValid;
    }

    private static IQueryable<Car> IncludeCarGraph(IQueryable<Car> cars) =>
        cars.Include(car => car.Driver)
            .ThenInclude(driver => driver!.Team)
            .Include(car => car.WheelSetup)
            .ThenInclude(tire => tire.Rim)
            .Include(car => car.Suspension);

    private async Task<bool> ApplyDriverOwnership(CarUpsertDto dto)
    {
        if (User.IsInRole("Admin")) return true;
        var driver = await GetCurrentDriverProfile();
        if (driver == null) return false;

        dto.DriverId = driver.Id;
        return true;
    }

    private async Task<bool> CanManageCar(Car car)
    {
        if (User.IsInRole("Admin")) return true;
        var driver = await GetCurrentDriverProfile();
        return driver != null && car.DriverId == driver.Id;
    }

    private async Task<Driver?> GetCurrentDriverProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId == null
            ? null
            : await _context.Drivers.FirstOrDefaultAsync(driver => driver.AppUserId == userId);
    }
}
