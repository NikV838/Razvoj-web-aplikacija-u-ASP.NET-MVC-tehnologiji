using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Repos
{
    public class RunEfRepository
    {
        private readonly SljemeTimeAttackDbContext _context;

        public RunEfRepository(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        public List<Run> GetAll()
        {
            return _context.Runs
                .Include(run => run.Driver)
                .Include(run => run.Car)
                .Include(run => run.Files)
                .ToList();
        }

        public Run? GetById(int id)
        {
            return _context.Runs
                .Include(run => run.Driver)
                .Include(run => run.Car)
                .Include(run => run.Files)
                .FirstOrDefault(run => run.Id == id);
        }

        public List<Run> Search(string? query)
        {
            var runs = _context.Runs
                .Include(run => run.Driver)
                .Include(run => run.Car)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var trimmedQuery = query.Trim();
                var matchedTracks = Enum.GetValues<SljemeTimeAttack.Enums.Track>()
                    .Where(track => track.ToString().Replace("_", " ").Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                var matchedDirections = Enum.GetValues<SljemeTimeAttack.Enums.DriveDirection>()
                    .Where(direction => direction.ToString().Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                var matchedWeather = Enum.GetValues<SljemeTimeAttack.Enums.WeatherCondition>()
                    .Where(weather => weather.ToString().Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                runs = runs.Where(run =>
                    run.Driver.Name.Contains(trimmedQuery) ||
                    run.Driver.Username.Contains(trimmedQuery) ||
                    run.Car.Make.Contains(trimmedQuery) ||
                    run.Car.Model.Contains(trimmedQuery) ||
                    run.Car.RegistrationNumber.Contains(trimmedQuery) ||
                    matchedTracks.Contains(run.Track) ||
                    matchedDirections.Contains(run.Direction) ||
                    matchedWeather.Contains(run.Weather));
            }

            return runs
                .OrderByDescending(run => run.Date)
                .ThenBy(run => run.BestTime)
                .ToList();
        }

        public void Add(Run run)
        {
            _context.Runs.Add(run);
            _context.SaveChanges();
        }

        public void Update(Run run)
        {
            _context.Runs.Update(run);
            _context.SaveChanges();
        }

        public void Delete(Run run)
        {
            _context.Runs.Remove(run);
            _context.SaveChanges();
        }
    }
}
