using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;

    public TeamsController(SljemeTimeAttackDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Teams.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(team =>
                team.Name.ToLower().Contains(term) ||
                team.Country.ToLower().Contains(term) ||
                (team.Sponsor != null && team.Sponsor.ToLower().Contains(term)));
        }

        return Ok((await query.OrderBy(team => team.Name).ToListAsync()).Select(team => team.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeamDto>> GetById(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        return team == null ? NotFound() : Ok(team.ToDto());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<TeamDto>> Create(TeamUpsertDto dto)
    {
        var team = new Team { Name = dto.Name, Country = dto.Country, Sponsor = dto.Sponsor, ImagePath = dto.ImagePath };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team.ToDto());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TeamUpsertDto dto)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team == null) return NotFound();

        team.Name = dto.Name;
        team.Country = dto.Country;
        team.Sponsor = dto.Sponsor;
        team.ImagePath = dto.ImagePath;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team == null) return NotFound();

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
