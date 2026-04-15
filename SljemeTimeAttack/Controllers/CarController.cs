using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class CarController : Controller
    {
        public IActionResult Index()
        {
            var cars = MockDataStore.Cars;
            return View(cars);
        }

        public IActionResult Details(int id)
        {
            var car = MockDataStore.Cars.FirstOrDefault(c => c.Id == id);
            if (car == null) return NotFound();
            return View(car);
        }
    }
}
