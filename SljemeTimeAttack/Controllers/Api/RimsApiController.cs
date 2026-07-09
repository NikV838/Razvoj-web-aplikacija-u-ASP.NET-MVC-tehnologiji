using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class RimsController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;

    public RimsController(SljemeTimeAttackDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RimDto>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Rims.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(rim =>
                rim.Make.ToLower().Contains(term) ||
                rim.Model.ToLower().Contains(term) ||
                rim.Material.ToLower().Contains(term));
        }

        return Ok((await query.OrderBy(rim => rim.Make).ThenBy(rim => rim.Model).ToListAsync()).Select(rim => rim.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RimDto>> GetById(int id)
    {
        var rim = await _context.Rims.FindAsync(id);
        return rim == null ? NotFound() : Ok(rim.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost]
    public async Task<ActionResult<RimDto>> Create(RimUpsertDto dto)
    {
        var rim = new Rim { Make = dto.Make, Model = dto.Model, SizeInJ = dto.SizeInJ, Material = dto.Material };
        _context.Rims.Add(rim);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = rim.Id }, rim.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RimUpsertDto dto)
    {
        var rim = await _context.Rims.FindAsync(id);
        if (rim == null) return NotFound();

        rim.Make = dto.Make;
        rim.Model = dto.Model;
        rim.SizeInJ = dto.SizeInJ;
        rim.Material = dto.Material;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var rim = await _context.Rims.FindAsync(id);
        if (rim == null) return NotFound();
        if (await _context.Tires.AnyAsync(tire => tire.RimId == id))
        {
            ModelState.AddModelError(string.Empty, "This rim is used by one or more tires.");
            return ValidationProblem(ModelState);
        }

        _context.Rims.Remove(rim);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
