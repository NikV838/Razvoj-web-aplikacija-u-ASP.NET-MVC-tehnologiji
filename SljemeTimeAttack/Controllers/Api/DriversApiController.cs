using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Services;

namespace SljemeTimeAttack.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;
    private readonly IGarageDeletionService _garageDeletionService;

    public DriversController(SljemeTimeAttackDbContext context, IGarageDeletionService garageDeletionService)
    {
        _context = context;
        _garageDeletionService = garageDeletionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Drivers.Include(driver => driver.Team).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(driver =>
                driver.Username.ToLower().Contains(term) ||
                driver.Name.ToLower().Contains(term) ||
                (driver.Email != null && driver.Email.ToLower().Contains(term)) ||
                (driver.Team != null && driver.Team.Name.ToLower().Contains(term)));
        }

        return Ok((await query.OrderBy(driver => driver.Name).ToListAsync()).Select(driver => driver.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DriverDto>> GetById(int id)
    {
        var driver = await _context.Drivers.Include(driver => driver.Team).FirstOrDefaultAsync(driver => driver.Id == id);
        return driver == null ? NotFound() : Ok(driver.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost]
    public async Task<ActionResult<DriverDto>> Create(DriverUpsertDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!User.IsInRole("Admin"))
        {
            if (currentUserId == null) return Forbid();
            if (await _context.Drivers.AnyAsync(driver => driver.AppUserId == currentUserId))
            {
                ModelState.AddModelError(string.Empty, "Current user already has a driver profile.");
                return ValidationProblem(ModelState);
            }
        }

        if (dto.TeamId.HasValue && !await _context.Teams.AnyAsync(team => team.Id == dto.TeamId.Value))
        {
            ModelState.AddModelError(nameof(dto.TeamId), "Team does not exist.");
            return ValidationProblem(ModelState);
        }

        var driver = new Driver
        {
            Username = dto.Username,
            Name = dto.Name,
            Age = dto.Age,
            YearsOfExperience = dto.YearsOfExperience,
            TeamId = dto.TeamId,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            AppUserId = User.IsInRole("Admin") ? null : currentUserId
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        await _context.Entry(driver).Reference(item => item.Team).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DriverUpsertDto dto)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null) return NotFound();
        if (!User.IsInRole("Admin") && driver.AppUserId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();

        if (dto.TeamId.HasValue && !await _context.Teams.AnyAsync(team => team.Id == dto.TeamId.Value))
        {
            ModelState.AddModelError(nameof(dto.TeamId), "Team does not exist.");
            return ValidationProblem(ModelState);
        }

        driver.Username = dto.Username;
        driver.Name = dto.Name;
        driver.Age = dto.Age;
        driver.YearsOfExperience = dto.YearsOfExperience;
        driver.TeamId = dto.TeamId;
        driver.Email = dto.Email;
        driver.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null) return NotFound();

        await _garageDeletionService.DeleteDriverAsync(driver, User.Identity?.Name);
        return NoContent();
    }
}
