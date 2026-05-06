using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class CarController : Controller
    {
        private readonly CarEfRepository _carRepository;

        public CarController(CarEfRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public IActionResult Index()
        {
            var cars = _carRepository.GetAll();
            return View(cars);
        }

        public IActionResult Details(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();
            return View(car);
        }
    }
}
