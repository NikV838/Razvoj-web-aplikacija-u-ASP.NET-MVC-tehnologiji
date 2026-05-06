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
    }
}
