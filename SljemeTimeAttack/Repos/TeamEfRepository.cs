using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Repos
{
    public class TeamEfRepository
    {
        private readonly SljemeTimeAttackDbContext _context;

        public TeamEfRepository(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        public List<Team> GetAll()
        {
            return _context.Teams
                .Include(team => team.Drivers)
                .ToList();
        }

        public Team? GetById(int id)
        {
            return _context.Teams
                .Include(team => team.Drivers)
                .FirstOrDefault(team => team.Id == id);
        }
    }
}
