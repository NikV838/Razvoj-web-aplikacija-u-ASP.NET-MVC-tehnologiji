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
                .ToList();
        }

        public Run? GetById(int id)
        {
            return _context.Runs
                .Include(run => run.Driver)
                .Include(run => run.Car)
                .FirstOrDefault(run => run.Id == id);
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
    }
}
