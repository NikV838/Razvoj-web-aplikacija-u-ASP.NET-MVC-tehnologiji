using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Repos
{
    public class CarEfRepository
    {
        private readonly SljemeTimeAttackDbContext _context;

        public CarEfRepository(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        public List<Car> GetAll()
        {
            return _context.Cars
                .Include(car => car.WheelSetup)
                    .ThenInclude(tire => tire.Rim)
                .Include(car => car.Suspension)
                .ToList();
        }

        public Car? GetById(int id)
        {
            return _context.Cars
                .Include(car => car.WheelSetup)
                    .ThenInclude(tire => tire.Rim)
                .Include(car => car.Suspension)
                .FirstOrDefault(car => car.Id == id);
        }
    }
}
