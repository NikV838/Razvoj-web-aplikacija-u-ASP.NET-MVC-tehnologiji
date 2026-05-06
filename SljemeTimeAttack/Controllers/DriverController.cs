using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class DriverController : Controller
    {
        private readonly DriverEfRepository _driverRepository;

        public DriverController(DriverEfRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        public IActionResult Index()
        {
            var drivers = _driverRepository.GetAll();
            return View(drivers);
        }

        public IActionResult Details(int id)
        {
            var driver = _driverRepository.GetById(id);
            if (driver == null) return NotFound();
            return View(driver);
        }
    }
}
