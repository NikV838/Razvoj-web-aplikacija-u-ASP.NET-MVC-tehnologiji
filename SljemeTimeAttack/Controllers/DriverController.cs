using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class DriverController : Controller
    {
        public IActionResult Index()
        {
            var drivers = MockDataStore.Drivers;
            return View(drivers);
        }

        public IActionResult Details(int id)
        {
            var driver = MockDataStore.Drivers.FirstOrDefault(d => d.Id == id);
            if (driver == null) return NotFound();
            return View(driver);
        }
    }
}
