using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class RunController : Controller
    {
        public IActionResult Index()
        {
            var runs = MockDataStore.Runs;
            return View(runs);
        }

        public IActionResult Details(int id)
        {
            var run = MockDataStore.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return NotFound();
            return View(run);
        }
    }
}
