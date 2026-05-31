using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                note.Run.Driver.Name.ToLower().Contains(term) ||
                note.Run.Car.Make.ToLower().Contains(term) ||
                note.Run.Car.Model.ToLower().Contains(term));
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
        if (!await _context.Runs.AnyAsync(run => run.Id == dto.RunId))
        {
            ModelState.AddModelError(nameof(dto.RunId), "Run does not exist.");
            return ValidationProblem(ModelState);
        }

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

        if (!await _context.Runs.AnyAsync(run => run.Id == dto.RunId))
        {
            ModelState.AddModelError(nameof(dto.RunId), "Run does not exist.");
            return ValidationProblem(ModelState);
        }

        note.RunId = dto.RunId;
        note.Note = dto.Note;
        note.CreatedDate = dto.CreatedDate ?? note.CreatedDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _context.RunNotes.FindAsync(id);
        if (note == null) return NotFound();

        _context.RunNotes.Remove(note);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static IQueryable<RunNote> IncludeRunNoteGraph(IQueryable<RunNote> notes) =>
        notes.Include(note => note.Run)
            .ThenInclude(run => run.Driver)
            .ThenInclude(driver => driver.Team)
            .Include(note => note.Run)
            .ThenInclude(run => run.Car)
            .ThenInclude(car => car.WheelSetup)
            .ThenInclude(tire => tire.Rim)
            .Include(note => note.Run)
            .ThenInclude(run => run.Car)
            .ThenInclude(car => car.Suspension)
            .Include(note => note.Run)
            .ThenInclude(run => run.Files);
}
