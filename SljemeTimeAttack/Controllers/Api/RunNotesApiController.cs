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
public class RunNotesController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;

    public RunNotesController(SljemeTimeAttackDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RunNoteDto>>> GetAll([FromQuery] string? search)
    {
        var query = IncludeRunNoteGraph(_context.RunNotes);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(note =>
                note.Note.ToLower().Contains(term) ||
                (note.Run.Driver != null && note.Run.Driver.Name.ToLower().Contains(term)) ||
                (note.Run.DriverNameSnapshot != null && note.Run.DriverNameSnapshot.ToLower().Contains(term)) ||
                (note.Run.Car != null && note.Run.Car.Make.ToLower().Contains(term)) ||
                (note.Run.Car != null && note.Run.Car.Model.ToLower().Contains(term)) ||
                (note.Run.CarDisplayNameSnapshot != null && note.Run.CarDisplayNameSnapshot.ToLower().Contains(term)));
        }

        return Ok((await query.OrderByDescending(note => note.CreatedDate).ToListAsync()).Select(note => note.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RunNoteDto>> GetById(int id)
    {
        var note = await IncludeRunNoteGraph(_context.RunNotes).FirstOrDefaultAsync(note => note.Id == id);
        return note == null ? NotFound() : Ok(note.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost]
    public async Task<ActionResult<RunNoteDto>> Create(RunNoteUpsertDto dto)
    {
        var run = await _context.Runs.FindAsync(dto.RunId);
        if (run == null)
        {
            ModelState.AddModelError(nameof(dto.RunId), "Run does not exist.");
            return ValidationProblem(ModelState);
        }
        if (!await CanManageRun(run)) return Forbid();

        var note = new RunNote
        {
            RunId = dto.RunId,
            Note = dto.Note,
            CreatedDate = dto.CreatedDate ?? DateTime.UtcNow
        };

        _context.RunNotes.Add(note);
        await _context.SaveChangesAsync();
        note = await IncludeRunNoteGraph(_context.RunNotes).FirstAsync(item => item.Id == note.Id);

        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RunNoteUpsertDto dto)
    {
        var note = await _context.RunNotes.FindAsync(id);
        if (note == null) return NotFound();
        var currentRun = await _context.Runs.FindAsync(note.RunId);
        if (currentRun == null) return NotFound();
        if (!await CanManageRun(currentRun)) return Forbid();

        var targetRun = await _context.Runs.FindAsync(dto.RunId);
        if (targetRun == null)
        {
            ModelState.AddModelError(nameof(dto.RunId), "Run does not exist.");
            return ValidationProblem(ModelState);
        }
        if (!await CanManageRun(targetRun)) return Forbid();

        note.RunId = dto.RunId;
        note.Note = dto.Note;
        note.CreatedDate = dto.CreatedDate ?? note.CreatedDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _context.RunNotes.FindAsync(id);
        if (note == null) return NotFound();
        var run = await _context.Runs.FindAsync(note.RunId);
        if (run == null) return NotFound();
        if (!await CanManageRun(run)) return Forbid();

        _context.RunNotes.Remove(note);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static IQueryable<RunNote> IncludeRunNoteGraph(IQueryable<RunNote> notes) =>
        notes.Include(note => note.Run)
            .ThenInclude(run => run.Driver)
            .ThenInclude(driver => driver!.Team)
            .Include(note => note.Run)
            .ThenInclude(run => run.Car)
            .ThenInclude(car => car!.WheelSetup)
            .ThenInclude(tire => tire.Rim)
            .Include(note => note.Run)
            .ThenInclude(run => run.Car)
            .ThenInclude(car => car!.Suspension)
            .Include(note => note.Run)
            .ThenInclude(run => run.Files);

    private async Task<bool> CanManageRun(Run run)
    {
        if (User.IsInRole("Admin")) return true;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;

        return run.DriverId.HasValue &&
            await _context.Drivers.AnyAsync(driver => driver.Id == run.DriverId.Value && driver.AppUserId == userId);
    }
}
