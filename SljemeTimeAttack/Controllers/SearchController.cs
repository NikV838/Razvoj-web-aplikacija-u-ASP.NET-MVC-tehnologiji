using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Enums;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class SearchController : Controller
    {
        private readonly SljemeTimeAttackDbContext _context;

        public SearchController(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        [HttpGet("/search")]
        public async Task<IActionResult> Index([FromQuery] string? q)
        {
            var viewModel = new SearchViewModel
            {
                Query = q?.Trim() ?? string.Empty
            };

            if (!viewModel.HasQuery)
            {
                return View(viewModel);
            }

            var term = viewModel.Query;
            var matchingTracks = Enum.GetValues<Track>()
                .Where(track => track.ToString().Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();

            viewModel.Cars = await _context.Cars
                .Include(car => car.Driver)
                .Where(car =>
                    car.Make.Contains(term) ||
                    car.Model.Contains(term) ||
                    car.RegistrationNumber.Contains(term))
                .OrderBy(car => car.Make)
                .ThenBy(car => car.Model)
                .Take(12)
                .ToListAsync();

            viewModel.Drivers = await _context.Drivers
                .Include(driver => driver.Team)
                .Where(driver =>
                    driver.Name.Contains(term) ||
                    driver.Username.Contains(term))
                .OrderBy(driver => driver.Name)
                .Take(12)
                .ToListAsync();

            viewModel.Teams = await _context.Teams
                .Include(team => team.Drivers)
                .Where(team =>
                    team.Name.Contains(term) ||
                    team.Country.Contains(term))
                .OrderBy(team => team.Name)
                .Take(12)
                .ToListAsync();

            viewModel.Runs = await _context.Runs
                .Include(run => run.Driver)
                .Include(run => run.Car)
                .Where(run =>
                    (run.Driver != null && run.Driver.Name.Contains(term)) ||
                    (run.DriverNameSnapshot != null && run.DriverNameSnapshot.Contains(term)) ||
                    (run.Car != null && run.Car.Make.Contains(term)) ||
                    (run.Car != null && run.Car.Model.Contains(term)) ||
                    (run.CarDisplayNameSnapshot != null && run.CarDisplayNameSnapshot.Contains(term)) ||
                    matchingTracks.Contains(run.Track))
                .OrderByDescending(run => run.Date)
                .Take(12)
                .ToListAsync();

            viewModel.Tires = await _context.Tires
                .Include(tire => tire.Rim)
                .Where(tire =>
                    tire.Brand.Contains(term) ||
                    tire.Model.Contains(term))
                .OrderBy(tire => tire.Brand)
                .ThenBy(tire => tire.Model)
                .Take(8)
                .ToListAsync();

            viewModel.Rims = await _context.Rims
                .Where(rim =>
                    rim.Make.Contains(term) ||
                    rim.Model.Contains(term))
                .OrderBy(rim => rim.Make)
                .ThenBy(rim => rim.Model)
                .Take(8)
                .ToListAsync();

            viewModel.Suspensions = await _context.Suspensions
                .Where(suspension => suspension.Brand.Contains(term))
                .OrderBy(suspension => suspension.Brand)
                .ThenBy(suspension => suspension.Type)
                .Take(8)
                .ToListAsync();

            return View(viewModel);
        }
    }
}
