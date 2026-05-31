using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class SuspensionsController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;

    public SuspensionsController(SljemeTimeAttackDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SuspensionDto>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Suspensions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(suspension =>
                suspension.Type.ToLower().Contains(term) ||
                suspension.Brand.ToLower().Contains(term));
        }

        return Ok((await query.OrderBy(suspension => suspension.Brand).ToListAsync()).Select(suspension => suspension.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SuspensionDto>> GetById(int id)
    {
        var suspension = await _context.Suspensions.FindAsync(id);
        return suspension == null ? NotFound() : Ok(suspension.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost]
    public async Task<ActionResult<SuspensionDto>> Create(SuspensionUpsertDto dto)
    {
        var suspension = new Suspension
        {
            Type = dto.Type,
            Brand = dto.Brand,
            HasFrontStrutBar = dto.HasFrontStrutBar,
            HasRearStrutBar = dto.HasRearStrutBar,
            RideHeightMm = dto.RideHeightMm,
            IsHeightAdjustable = dto.IsHeightAdjustable,
            IsStiffnessAdjustable = dto.IsStiffnessAdjustable,
            FrontStiffness = dto.FrontStiffness,
            RearStiffness = dto.RearStiffness
        };

        _context.Suspensions.Add(suspension);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = suspension.Id }, suspension.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SuspensionUpsertDto dto)
    {
        var suspension = await _context.Suspensions.FindAsync(id);
        if (suspension == null) return NotFound();

        suspension.Type = dto.Type;
        suspension.Brand = dto.Brand;
        suspension.HasFrontStrutBar = dto.HasFrontStrutBar;
        suspension.HasRearStrutBar = dto.HasRearStrutBar;
        suspension.RideHeightMm = dto.RideHeightMm;
        suspension.IsHeightAdjustable = dto.IsHeightAdjustable;
        suspension.IsStiffnessAdjustable = dto.IsStiffnessAdjustable;
        suspension.FrontStiffness = dto.FrontStiffness;
        suspension.RearStiffness = dto.RearStiffness;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var suspension = await _context.Suspensions.FindAsync(id);
        if (suspension == null) return NotFound();

        _context.Suspensions.Remove(suspension);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
