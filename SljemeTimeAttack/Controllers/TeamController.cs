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
        private readonly IWebHostEnvironment _environment;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif"
        };

        public TeamController(TeamEfRepository teamRepository, IWebHostEnvironment environment)
        {
            _teamRepository = teamRepository;
            _environment = environment;
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
        public async Task<IActionResult> Create(TeamCreateViewModel viewModel)
        {
            ValidateImageFile(viewModel);
            if (!ModelState.IsValid) return View(viewModel);

            var team = new Team
            {
                Name = viewModel.Name,
                Country = viewModel.Country,
                Sponsor = viewModel.Sponsor,
                ImagePath = await SaveTeamImageAsync(viewModel.ImageFile)
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

        private void ValidateImageFile(TeamCreateViewModel viewModel)
        {
            if (viewModel.ImageFile == null || viewModel.ImageFile.Length == 0)
            {
                return;
            }

            var extension = Path.GetExtension(viewModel.ImageFile.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(TeamCreateViewModel.ImageFile), "Upload a JPG, PNG, WEBP, or GIF image.");
            }
        }

        private async Task<string?> SaveTeamImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var uploadRoot = Path.Combine(_environment.WebRootPath, "img", "teams");
            Directory.CreateDirectory(uploadRoot);
            var physicalPath = Path.Combine(uploadRoot, fileName);

            await using var stream = System.IO.File.Create(physicalPath);
            await file.CopyToAsync(stream);

            return $"/img/teams/{fileName}";
        }
    }
}
