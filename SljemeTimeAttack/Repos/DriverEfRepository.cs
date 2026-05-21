using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Repos
{
    public class DriverEfRepository
    {
        private readonly SljemeTimeAttackDbContext _context;

        public DriverEfRepository(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        public List<Driver> GetAll()
        {
            return _context.Drivers
                .Include(driver => driver.Team)
                .Include(driver => driver.CarsOwned)
                .Include(driver => driver.Runs)
                .ToList();
        }

        public Driver? GetById(int id)
        {
            return _context.Drivers
                .Include(driver => driver.Team)
                .Include(driver => driver.CarsOwned)
                .Include(driver => driver.Runs)
                .FirstOrDefault(driver => driver.Id == id);
        }

        public List<Driver> Search(string? query)
        {
            var drivers = _context.Drivers
                .Include(driver => driver.Team)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var trimmedQuery = query.Trim();
                drivers = drivers.Where(driver =>
                    driver.Username.Contains(trimmedQuery) ||
                    driver.Name.Contains(trimmedQuery) ||
                    (driver.Email != null && driver.Email.Contains(trimmedQuery)));
            }

            return drivers
                .OrderBy(driver => driver.Name)
                .ToList();
        }

        public void Add(Driver driver)
        {
            _context.Drivers.Add(driver);
            _context.SaveChanges();
        }

        public void Update(Driver driver)
        {
            _context.Drivers.Update(driver);
            _context.SaveChanges();
        }

        public void Delete(Driver driver)
        {
            _context.Drivers.Remove(driver);
            _context.SaveChanges();
        }
    }
}
