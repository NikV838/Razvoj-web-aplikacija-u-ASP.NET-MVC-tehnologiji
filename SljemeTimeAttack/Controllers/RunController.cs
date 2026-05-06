using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class RunController : Controller
    {
        private readonly RunEfRepository _runRepository;

        public RunController(RunEfRepository runRepository)
        {
            _runRepository = runRepository;
        }

        public IActionResult Index()
        {
            var runs = _runRepository.GetAll();
            return View(runs);
        }

        public IActionResult Details(int id)
        {
            var run = _runRepository.GetById(id);
            if (run == null) return NotFound();
            return View(run);
        }
    }
}
