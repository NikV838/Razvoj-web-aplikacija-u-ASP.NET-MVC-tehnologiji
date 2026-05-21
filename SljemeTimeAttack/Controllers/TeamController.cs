using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Repos;

namespace SljemeTimeAttack.Controllers
{
    public class TeamController : Controller
    {
        private readonly TeamEfRepository _teamRepository;

        public TeamController(TeamEfRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public IActionResult Index()
        {
            var teams = _teamRepository.GetAll();
            return View(teams);
        }

        public IActionResult Details(int id)
        {
            var team = _teamRepository.GetById(id);
            if (team == null) return NotFound();
            return View(team);
        }

        public IActionResult Search(string? query)
        {
            var teams = _teamRepository.Search(query)
                .Select(team => new
                {
                    id = team.Id,
                    text = team.Name,
                    meta = team.Country
                });

            return Json(teams);
        }
    }
}
