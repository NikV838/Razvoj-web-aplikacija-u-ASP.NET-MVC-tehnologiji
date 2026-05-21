using Microsoft.AspNetCore.Mvc;
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

        public RunController(
            RunEfRepository runRepository,
            DriverEfRepository driverRepository,
            CarEfRepository carRepository)
        {
            _runRepository = runRepository;
            _driverRepository = driverRepository;
            _carRepository = carRepository;
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

        public IActionResult Create()
        {
            var viewModel = new RunCreateViewModel
            {
                Date = DateTime.Now
            };
            PopulateRunOptions(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RunCreateViewModel viewModel)
        {
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

        public IActionResult Edit(int id)
        {
            var run = _runRepository.GetById(id);
            if (run == null) return NotFound();

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
            PopulateRunOptions(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RunEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

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

            var run = _runRepository.GetById(id);
            if (run == null) return NotFound();

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

        private void PopulateRunOptions(RunFormViewModel viewModel)
        {
            viewModel.DriverOptions = _driverRepository.GetAll()
                .OrderBy(driver => driver.Name)
                .Select(driver => new SelectListItem(driver.Name, driver.Id.ToString()))
                .ToList();

            viewModel.CarOptions = _carRepository.GetAll()
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
    }
}
