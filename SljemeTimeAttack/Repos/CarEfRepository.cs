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
                .Include(car => car.Driver)
                .Include(car => car.WheelSetup)
                    .ThenInclude(tire => tire.Rim)
                .Include(car => car.Suspension)
                .ToList();
        }

        public Car? GetById(int id)
        {
            return _context.Cars
                .Include(car => car.Driver)
                .Include(car => car.WheelSetup)
                    .ThenInclude(tire => tire.Rim)
                .Include(car => car.Suspension)
                .FirstOrDefault(car => car.Id == id);
        }

        public List<Car> Search(string? query)
        {
            var cars = _context.Cars
                .Include(car => car.Driver)
                .Include(car => car.WheelSetup)
                    .ThenInclude(tire => tire.Rim)
                .Include(car => car.Suspension)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var trimmedQuery = query.Trim();
                cars = cars.Where(car =>
                    car.Make.Contains(trimmedQuery) ||
                    car.Model.Contains(trimmedQuery) ||
                    car.RegistrationNumber.Contains(trimmedQuery) ||
                    (car.Driver != null && car.Driver.Name.Contains(trimmedQuery)));
            }

            return cars
                .OrderBy(car => car.Make)
                .ThenBy(car => car.Model)
                .ToList();
        }

        public List<Tire> GetTires()
        {
            return _context.Tires
                .Include(tire => tire.Rim)
                .OrderBy(tire => tire.Brand)
                .ThenBy(tire => tire.Model)
                .ToList();
        }

        public Tire? GetTireById(int id)
        {
            return _context.Tires
                .Include(tire => tire.Rim)
                .FirstOrDefault(tire => tire.Id == id);
        }

        public List<Suspension> GetSuspensions()
        {
            return _context.Suspensions
                .OrderBy(suspension => suspension.Brand)
                .ThenBy(suspension => suspension.Type)
                .ToList();
        }

        public Suspension? GetSuspensionById(int id)
        {
            return _context.Suspensions.FirstOrDefault(suspension => suspension.Id == id);
        }

        public void Add(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
        }

        public void Update(Car car)
        {
            _context.Cars.Update(car);
            _context.SaveChanges();
        }

        public void Delete(Car car)
        {
            _context.Cars.Remove(car);
            _context.SaveChanges();
        }
    }
}
