using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;
using SljemeTimeAttack.ViewModels;

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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new TeamCreateViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TeamCreateViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var team = new Team
            {
                Name = viewModel.Name,
                Country = viewModel.Country,
                Sponsor = viewModel.Sponsor
            };

            _teamRepository.Add(team);
            return RedirectToAction(nameof(Details), new { id = team.Id });
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
