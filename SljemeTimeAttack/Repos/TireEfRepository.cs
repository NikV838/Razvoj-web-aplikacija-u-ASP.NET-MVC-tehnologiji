using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Repos
{
    public class TireEfRepository
    {
        private readonly SljemeTimeAttackDbContext _context;

        public TireEfRepository(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        public Rim? GetRimById(int id)
        {
            return _context.Rims.FirstOrDefault(rim => rim.Id == id);
        }

        public List<Rim> GetRims()
        {
            return _context.Rims
                .OrderBy(rim => rim.Make)
                .ThenBy(rim => rim.Model)
                .ToList();
        }

        public void Add(Tire tire)
        {
            _context.Tires.Add(tire);
            _context.SaveChanges();
        }
    }
}
