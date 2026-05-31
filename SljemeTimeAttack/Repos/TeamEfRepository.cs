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

        public List<Team> Search(string? query)
        {
            var teams = _context.Teams.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var trimmedQuery = query.Trim();
                teams = teams.Where(team => team.Name.Contains(trimmedQuery));
            }

            return teams
                .OrderBy(team => team.Name)
                .Take(8)
                .ToList();
        }

        public void Add(Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();
        }
    }
}
