using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Dtos;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class RunsController : ControllerBase
{
    private readonly SljemeTimeAttackDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public RunsController(SljemeTimeAttackDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RunDto>>> GetAll([FromQuery] string? search)
    {
        var query = IncludeRunGraph(_context.Runs);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(run =>
                run.Driver.Name.ToLower().Contains(term) ||
                run.Driver.Username.ToLower().Contains(term) ||
                run.Car.Make.ToLower().Contains(term) ||
                run.Car.Model.ToLower().Contains(term) ||
                run.Car.RegistrationNumber.ToLower().Contains(term));
        }

        return Ok((await query.OrderByDescending(run => run.Date).ToListAsync()).Select(run => run.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RunDto>> GetById(int id)
    {
        var run = await IncludeRunGraph(_context.Runs).FirstOrDefaultAsync(run => run.Id == id);
        return run == null ? NotFound() : Ok(run.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost]
    public async Task<ActionResult<RunDto>> Create(RunUpsertDto dto)
    {
        if (!await ReferencesExist(dto)) return ValidationProblem(ModelState);

        var run = new Run
        {
            DriverId = dto.DriverId,
            CarId = dto.CarId,
            Track = dto.Track,
            BestTime = dto.BestTime!.Value,
            Date = dto.Date!.Value,
            Direction = dto.Direction,
            Weather = dto.Weather
        };

        _context.Runs.Add(run);
        await _context.SaveChangesAsync();
        run = await IncludeRunGraph(_context.Runs).FirstAsync(item => item.Id == run.Id);

        return CreatedAtAction(nameof(GetById), new { id = run.Id }, run.ToDto());
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RunUpsertDto dto)
    {
        var run = await _context.Runs.FindAsync(id);
        if (run == null) return NotFound();
        if (!await ReferencesExist(dto)) return ValidationProblem(ModelState);

        run.DriverId = dto.DriverId;
        run.CarId = dto.CarId;
        run.Track = dto.Track;
        run.BestTime = dto.BestTime!.Value;
        run.Date = dto.Date!.Value;
        run.Direction = dto.Direction;
        run.Weather = dto.Weather;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var run = await _context.Runs.FindAsync(id);
        if (run == null) return NotFound();

        _context.Runs.Remove(run);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpPost("{runId:int}/files")]
    public async Task<ActionResult<RunFileDto>> UploadFile(int runId, IFormFile file)
    {
        if (!await _context.Runs.AnyAsync(run => run.Id == runId)) return NotFound();
        if (file.Length == 0) return BadRequest("Uploaded file is empty.");

        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var uploadsRoot = Path.Combine(GetWebRootPath(), "uploads", "runs", runId.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var physicalPath = Path.Combine(uploadsRoot, storedFileName);
        await using (var stream = System.IO.File.Create(physicalPath))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/runs/{runId}/{storedFileName}";
        var runFile = new RunFile
        {
            RunId = runId,
            OriginalFileName = Path.GetFileName(file.FileName),
            StoredFileName = storedFileName,
            ContentType = file.ContentType,
            FilePath = relativePath,
            UploadedAt = DateTime.UtcNow
        };

        _context.RunFiles.Add(runFile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFiles), new { runId }, runFile.ToDto());
    }

    [HttpGet("{runId:int}/files")]
    public async Task<ActionResult<IEnumerable<RunFileDto>>> GetFiles(int runId)
    {
        if (!await _context.Runs.AnyAsync(run => run.Id == runId)) return NotFound();

        var files = await _context.RunFiles
            .Where(file => file.RunId == runId)
            .OrderByDescending(file => file.UploadedAt)
            .ToListAsync();

        return Ok(files.Select(file => file.ToDto()));
    }

    [Authorize(Roles = "Admin,User,Racer")]
    [HttpDelete("files/{fileId:int}")]
    public async Task<IActionResult> DeleteFile(int fileId)
    {
        var runFile = await _context.RunFiles.FindAsync(fileId);
        if (runFile == null) return NotFound();

        var physicalPath = Path.Combine(GetWebRootPath(), runFile.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        _context.RunFiles.Remove(runFile);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> ReferencesExist(RunUpsertDto dto)
    {
        var isValid = true;
        if (!await _context.Drivers.AnyAsync(driver => driver.Id == dto.DriverId))
        {
            ModelState.AddModelError(nameof(dto.DriverId), "Driver does not exist.");
            isValid = false;
        }

        if (!await _context.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car does not exist.");
            isValid = false;
        }

        return isValid;
    }

    private string GetWebRootPath() =>
        _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

    private static IQueryable<Run> IncludeRunGraph(IQueryable<Run> runs) =>
        runs.Include(run => run.Driver)
            .ThenInclude(driver => driver.Team)
            .Include(run => run.Car)
            .ThenInclude(car => car.WheelSetup)
            .ThenInclude(tire => tire.Rim)
            .Include(run => run.Car)
            .ThenInclude(car => car.Suspension)
            .Include(run => run.Files);
}
