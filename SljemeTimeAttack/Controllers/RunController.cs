using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SljemeTimeAttack.Enums;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class RunController : Controller
    {
        private readonly RunEfRepository _runRepository;
        private readonly DriverEfRepository _driverRepository;
        private readonly CarEfRepository _carRepository;
        private readonly UserManager<AppUser> _userManager;

        public RunController(
            RunEfRepository runRepository,
            DriverEfRepository driverRepository,
            CarEfRepository carRepository,
            UserManager<AppUser> userManager)
        {
            _runRepository = runRepository;
            _driverRepository = driverRepository;
            _carRepository = carRepository;
            _userManager = userManager;
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

        public IActionResult Search(string? query)
        {
            var isAuthenticated = User.Identity?.IsAuthenticated == true;
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            var runs = _runRepository.Search(query)
                .Select(run => new
                {
                    id = run.Id,
                    track = FormatEnum(run.Track.ToString()),
                    driverName = run.Driver.Name,
                    carName = $"{run.Car.Make} {run.Car.Model}",
                    registrationNumber = run.Car.RegistrationNumber,
                    bestTime = run.BestTime.ToString(@"m\:ss"),
                    direction = run.Direction.ToString(),
                    weather = run.Weather.ToString(),
                    date = run.Date.ToString("dd.MM.yyyy. HH:mm"),
                    detailsUrl = Url.Action(nameof(Details), new { id = run.Id }),
                    editUrl = Url.Action(nameof(Edit), new { id = run.Id }),
                    deleteUrl = Url.Action(nameof(Delete), new { id = run.Id }),
                    canManage = isAdmin || (isAuthenticated && run.Driver.AppUserId == currentUserId)
                });

            return Json(runs);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var viewModel = new RunCreateViewModel
            {
                Date = DateTime.Now
            };
            await ApplyRunOwnershipDefaults(viewModel);
            PopulateRunOptions(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(RunCreateViewModel viewModel)
        {
            await ApplyRunOwnershipDefaults(viewModel);
            ValidateRunReferences(viewModel);

            if (!TryGetBestTime(viewModel.BestTime, out var bestTime))
            {
                ModelState.AddModelError(nameof(RunFormViewModel.BestTime), "Use minutes:seconds, for example 5:20.");
            }

            if (!ModelState.IsValid || bestTime == null)
            {
                PopulateRunOptions(viewModel);
                return View(viewModel);
            }

            var run = new Run
            {
                DriverId = viewModel.DriverId!.Value,
                CarId = viewModel.CarId!.Value,
                Track = viewModel.Track!.Value,
                BestTime = bestTime.Value,
                Date = viewModel.Date!.Value,
                Direction = viewModel.Direction!.Value,
                Weather = viewModel.Weather!.Value
            };

            _runRepository.Add(run);
            return RedirectToAction(nameof(Details), new { id = run.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var run = _runRepository.GetById(id);
            if (run == null) return NotFound();
            if (!await CanManageRun(run)) return Forbid();

            var viewModel = new RunEditViewModel
            {
                Id = run.Id,
                DriverId = run.DriverId,
                CarId = run.CarId,
                Track = run.Track,
                BestTime = run.BestTime.ToString(@"m\:ss"),
                Date = run.Date,
                Direction = run.Direction,
                Weather = run.Weather
            };
            viewModel.CanChooseDriver = User.IsInRole("Admin");
            PopulateRunOptions(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, RunEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();
            var run = _runRepository.GetById(id);
            if (run == null) return NotFound();
            if (!await CanManageRun(run)) return Forbid();

            await ApplyRunOwnershipDefaults(viewModel);

            ValidateRunReferences(viewModel);

            if (!TryGetBestTime(viewModel.BestTime, out var bestTime))
            {
                ModelState.AddModelError(nameof(RunFormViewModel.BestTime), "Use minutes:seconds, for example 5:20.");
            }

            if (!ModelState.IsValid || bestTime == null)
            {
                PopulateRunOptions(viewModel);
                return View(viewModel);
            }

            run.DriverId = viewModel.DriverId!.Value;
            run.CarId = viewModel.CarId!.Value;
            run.Track = viewModel.Track!.Value;
            run.BestTime = bestTime.Value;
            run.Date = viewModel.Date!.Value;
            run.Direction = viewModel.Direction!.Value;
            run.Weather = viewModel.Weather!.Value;

            _runRepository.Update(run);
            return RedirectToAction(nameof(Details), new { id = run.Id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var run = _runRepository.GetById(id);
            if (run == null) return NotFound();
            if (!await CanManageRun(run)) return Forbid();

            return View(new RunDeleteViewModel
            {
                Id = run.Id,
                Track = FormatEnum(run.Track.ToString()),
                DriverName = run.Driver.Name,
                CarName = $"{run.Car.Make} {run.Car.Model}",
                Date = run.Date,
                BestTime = run.BestTime.ToString(@"m\:ss")
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var run = _runRepository.GetById(id);
            if (run == null) return NotFound();
            if (!await CanManageRun(run)) return Forbid();

            _runRepository.Delete(run);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateRunOptions(RunFormViewModel viewModel)
        {
            var currentDriverId = viewModel.CanChooseDriver ? null : viewModel.DriverId;

            viewModel.DriverOptions = _driverRepository.GetAll()
                .Where(driver => viewModel.CanChooseDriver || driver.Id == currentDriverId)
                .OrderBy(driver => driver.Name)
                .Select(driver => new SelectListItem(driver.Name, driver.Id.ToString()))
                .ToList();

            viewModel.CarOptions = _carRepository.GetAll()
                .Where(car => viewModel.CanChooseDriver || car.DriverId == currentDriverId)
                .OrderBy(car => car.Make)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Make} {car.Model} ({car.RegistrationNumber})", car.Id.ToString()))
                .ToList();

            viewModel.TrackOptions = GetEnumOptions<Track>();
            viewModel.DirectionOptions = GetEnumOptions<DriveDirection>();
            viewModel.WeatherOptions = GetEnumOptions<WeatherCondition>();
        }

        private void ValidateRunReferences(RunFormViewModel viewModel)
        {
            if (viewModel.DriverId.HasValue && _driverRepository.GetById(viewModel.DriverId.Value) == null)
            {
                ModelState.AddModelError(nameof(RunFormViewModel.DriverId), "Select an existing driver.");
            }

            if (viewModel.CarId.HasValue && _carRepository.GetById(viewModel.CarId.Value) == null)
            {
                ModelState.AddModelError(nameof(RunFormViewModel.CarId), "Select an existing car.");
            }

            if (!viewModel.CanChooseDriver && viewModel.CarId.HasValue)
            {
                var car = _carRepository.GetById(viewModel.CarId.Value);
                if (car == null || car.DriverId != viewModel.DriverId)
                {
                    ModelState.AddModelError(nameof(RunFormViewModel.CarId), "Select one of your cars.");
                }
            }
        }

        private async Task ApplyRunOwnershipDefaults(RunFormViewModel viewModel)
        {
            viewModel.CanChooseDriver = User.IsInRole("Admin");
            if (viewModel.CanChooseDriver) return;

            var driver = await GetCurrentDriverProfile();
            if (driver == null)
            {
                ModelState.AddModelError(string.Empty, "Your account does not have a linked driver profile.");
                return;
            }

            viewModel.DriverId = driver.Id;
        }

        private async Task<bool> CanManageRun(Run run)
        {
            if (User.IsInRole("Admin")) return true;
            var driver = await GetCurrentDriverProfile();
            return driver != null && run.DriverId == driver.Id;
        }

        private async Task<Driver?> GetCurrentDriverProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            return user == null
                ? null
                : _driverRepository.GetAll().FirstOrDefault(driver => driver.AppUserId == user.Id);
        }

        private static List<SelectListItem> GetEnumOptions<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                .Select(value => new SelectListItem(value.ToString(), Convert.ToInt32(value).ToString()))
                .ToList();
        }

        private static bool TryGetBestTime(string value, out TimeSpan? bestTime)
        {
            bestTime = null;
            var parts = value.Split(':');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out var minutes) ||
                !int.TryParse(parts[1], out var seconds) ||
                minutes < 0 ||
                seconds < 0 ||
                seconds > 59)
            {
                return false;
            }

            bestTime = TimeSpan.FromMinutes(minutes).Add(TimeSpan.FromSeconds(seconds));
            return true;
        }

        private static string FormatEnum(string value)
        {
            return value.Replace("_", " ");
        }
    }
}
