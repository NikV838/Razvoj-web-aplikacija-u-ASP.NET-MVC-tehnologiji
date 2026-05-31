using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TiresController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;

    public TiresController(SljemeTimeAttackDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TireDto>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Tires.Include(tire => tire.Rim).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(tire =>
                tire.Brand.ToLower().Contains(term) ||
                tire.Model.ToLower().Contains(term) ||
                tire.Type.ToLower().Contains(term) ||
                tire.Dot.ToLower().Contains(term) ||
                tire.Rim.Make.ToLower().Contains(term) ||
                tire.Rim.Model.ToLower().Contains(term));
        }

        return Ok((await query.OrderBy(tire => tire.Brand).ThenBy(tire => tire.Model).ToListAsync()).Select(tire => tire.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TireDto>> GetById(int id)
    {
        var tire = await _context.Tires.Include(tire => tire.Rim).FirstOrDefaultAsync(tire => tire.Id == id);
        return tire == null ? NotFound() : Ok(tire.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost]
    public async Task<ActionResult<TireDto>> Create(TireUpsertDto dto)
    {
        if (!await _context.Rims.AnyAsync(rim => rim.Id == dto.RimId))
        {
            ModelState.AddModelError(nameof(dto.RimId), "Rim does not exist.");
            return ValidationProblem(ModelState);
        }

        var tire = new Tire
        {
            Brand = dto.Brand,
            Model = dto.Model,
            Type = dto.Type,
            SizeInMm = dto.SizeInMm,
            Dot = dto.Dot,
            RimId = dto.RimId
        };

        _context.Tires.Add(tire);
        await _context.SaveChangesAsync();
        await _context.Entry(tire).Reference(item => item.Rim).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = tire.Id }, tire.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TireUpsertDto dto)
    {
        var tire = await _context.Tires.FindAsync(id);
        if (tire == null) return NotFound();

        if (!await _context.Rims.AnyAsync(rim => rim.Id == dto.RimId))
        {
            ModelState.AddModelError(nameof(dto.RimId), "Rim does not exist.");
            return ValidationProblem(ModelState);
        }

        tire.Brand = dto.Brand;
        tire.Model = dto.Model;
        tire.Type = dto.Type;
        tire.SizeInMm = dto.SizeInMm;
        tire.Dot = dto.Dot;
        tire.RimId = dto.RimId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tire = await _context.Tires.FindAsync(id);
        if (tire == null) return NotFound();

        _context.Tires.Remove(tire);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
