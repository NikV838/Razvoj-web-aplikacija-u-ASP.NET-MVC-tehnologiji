using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class DriverController : Controller
    {
        private readonly DriverEfRepository _driverRepository;
        private readonly TeamEfRepository _teamRepository;
        private readonly UserManager<AppUser> _userManager;

        public DriverController(
            DriverEfRepository driverRepository,
            TeamEfRepository teamRepository,
            UserManager<AppUser> userManager)
        {
            _driverRepository = driverRepository;
            _teamRepository = teamRepository;
            _userManager = userManager;
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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new DriverCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(DriverCreateViewModel viewModel)
        {
            ValidateSelectedTeam(viewModel.TeamId);

            if (!ModelState.IsValid)
            {
                viewModel.TeamName = GetTeamName(viewModel.TeamId);
                return View(viewModel);
            }

            var driver = new Driver
            {
                Username = viewModel.Username,
                Name = viewModel.Name,
                Age = viewModel.Age,
                YearsOfExperience = viewModel.YearsOfExperience,
                TeamId = viewModel.TeamId,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber
            };

            _driverRepository.Add(driver);
            return RedirectToAction(nameof(Details), new { id = driver.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var driver = _driverRepository.GetById(id);
            if (driver == null) return NotFound();
            if (!await CanManageDriver(driver)) return Forbid();

            var viewModel = new DriverEditViewModel
            {
                Id = driver.Id,
                Username = driver.Username,
                Name = driver.Name,
                Age = driver.Age,
                YearsOfExperience = driver.YearsOfExperience,
                TeamId = driver.TeamId,
                TeamName = driver.Team?.Name ?? string.Empty,
                Email = driver.Email,
                PhoneNumber = driver.PhoneNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, DriverEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();
            var driver = _driverRepository.GetById(id);
            if (driver == null) return NotFound();
            if (!await CanManageDriver(driver)) return Forbid();

            ValidateSelectedTeam(viewModel.TeamId);

            if (!ModelState.IsValid)
            {
                viewModel.TeamName = GetTeamName(viewModel.TeamId);
                return View(viewModel);
            }

            driver.Username = viewModel.Username;
            driver.Name = viewModel.Name;
            driver.Age = viewModel.Age;
            driver.YearsOfExperience = viewModel.YearsOfExperience;
            driver.TeamId = viewModel.TeamId;
            driver.Email = viewModel.Email;
            driver.PhoneNumber = viewModel.PhoneNumber;

            _driverRepository.Update(driver);
            return RedirectToAction(nameof(Details), new { id = driver.Id });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var driver = _driverRepository.GetById(id);
            if (driver == null) return NotFound();

            var viewModel = new DriverDeleteViewModel
            {
                Id = driver.Id,
                Username = driver.Username,
                Name = driver.Name,
                TeamName = driver.Team?.Name
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var driver = _driverRepository.GetById(id);
            if (driver == null) return NotFound();

            _driverRepository.Delete(driver);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search(string? query)
        {
            var isAuthenticated = User.Identity?.IsAuthenticated == true;
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            var drivers = _driverRepository.Search(query);
            var results = drivers.Select(driver => new
            {
                id = driver.Id,
                name = driver.Name,
                username = driver.Username,
                age = driver.Age,
                yearsOfExperience = driver.YearsOfExperience,
                teamName = driver.Team?.Name ?? "Independent",
                email = driver.Email ?? string.Empty,
                detailsUrl = Url.Action(nameof(Details), new { id = driver.Id }),
                editUrl = Url.Action(nameof(Edit), new { id = driver.Id }),
                deleteUrl = Url.Action(nameof(Delete), new { id = driver.Id }),
                canManage = isAdmin || (isAuthenticated && driver.AppUserId == currentUserId),
                canDelete = isAdmin
            });

            return Json(results);
        }

        public IActionResult Lookup(string? query)
        {
            var drivers = _driverRepository.Search(query)
                .Take(8)
                .Select(driver => new
                {
                    id = driver.Id,
                    text = driver.Name,
                    meta = driver.Username
                });

            return Json(drivers);
        }

        private string GetTeamName(int? teamId)
        {
            return teamId.HasValue
                ? _teamRepository.GetById(teamId.Value)?.Name ?? string.Empty
                : string.Empty;
        }

        private void ValidateSelectedTeam(int? teamId)
        {
            if (teamId.HasValue && _teamRepository.GetById(teamId.Value) == null)
            {
                ModelState.AddModelError(nameof(DriverCreateViewModel.TeamId), "Select an existing team.");
            }
        }

        private async Task<bool> CanManageDriver(Driver driver)
        {
            if (User.IsInRole("Admin")) return true;
            var user = await _userManager.GetUserAsync(User);
            return user != null && driver.AppUserId == user.Id;
        }
    }
}
