using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index()
        {
            var teams = MockDataStore.Teams;
            return View(teams);
        }

        public IActionResult Details(int id)
        {
            var team = MockDataStore.Teams.FirstOrDefault(t => t.Id == id);
            if (team == null) return NotFound();
            return View(team);
        }
    }
}
