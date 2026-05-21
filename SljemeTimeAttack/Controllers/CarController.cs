using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class CarController : Controller
    {
        private readonly CarEfRepository _carRepository;
        private readonly DriverEfRepository _driverRepository;
        private readonly TireEfRepository _tireRepository;

        public CarController(
            CarEfRepository carRepository,
            DriverEfRepository driverRepository,
            TireEfRepository tireRepository)
        {
            _carRepository = carRepository;
            _driverRepository = driverRepository;
            _tireRepository = tireRepository;
        }

        public IActionResult Index()
        {
            var cars = _carRepository.GetAll();
            return View(cars);
        }

        public IActionResult Details(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();
            return View(car);
        }

        public IActionResult Create()
        {
            var viewModel = new CarCreateViewModel();
            PopulateCarOptions(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CarCreateViewModel viewModel)
        {
            ValidateCarReferences(viewModel);

            if (!ModelState.IsValid)
            {
                PopulateCarOptions(viewModel);
                PopulateDriverName(viewModel);
                return View(viewModel);
            }

            var car = new Car
            {
                Make = viewModel.Make,
                Model = viewModel.Model,
                Horsepower = viewModel.Horsepower,
                WeightKg = viewModel.WeightKg,
                Year = viewModel.Year,
                RegistrationNumber = viewModel.RegistrationNumber,
                DriverId = viewModel.DriverId,
                TireId = viewModel.TireId!.Value,
                SuspensionId = viewModel.SuspensionId!.Value
            };

            _carRepository.Add(car);
            return RedirectToAction(nameof(Details), new { id = car.Id });
        }

        public IActionResult Edit(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();

            var viewModel = new CarEditViewModel
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Horsepower = car.Horsepower,
                WeightKg = car.WeightKg,
                Year = car.Year,
                RegistrationNumber = car.RegistrationNumber,
                DriverId = car.DriverId,
                DriverName = car.Driver?.Name ?? string.Empty,
                TireId = car.TireId,
                SuspensionId = car.SuspensionId
            };
            PopulateCarOptions(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CarEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            ValidateCarReferences(viewModel);

            if (!ModelState.IsValid)
            {
                PopulateCarOptions(viewModel);
                PopulateDriverName(viewModel);
                return View(viewModel);
            }

            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();

            car.Make = viewModel.Make;
            car.Model = viewModel.Model;
            car.Horsepower = viewModel.Horsepower;
            car.WeightKg = viewModel.WeightKg;
            car.Year = viewModel.Year;
            car.RegistrationNumber = viewModel.RegistrationNumber;
            car.DriverId = viewModel.DriverId;
            car.TireId = viewModel.TireId!.Value;
            car.SuspensionId = viewModel.SuspensionId!.Value;

            _carRepository.Update(car);
            return RedirectToAction(nameof(Details), new { id = car.Id });
        }

        public IActionResult Delete(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();

            return View(new CarDeleteViewModel
            {
                Id = car.Id,
                Name = $"{car.Make} {car.Model}",
                RegistrationNumber = car.RegistrationNumber,
                DriverName = car.Driver?.Name
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();

            _carRepository.Delete(car);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search(string? query)
        {
            var cars = _carRepository.Search(query)
                .Select(car => new
                {
                    id = car.Id,
                    make = car.Make,
                    model = car.Model,
                    registrationNumber = car.RegistrationNumber,
                    horsepower = car.Horsepower,
                    weightKg = car.WeightKg,
                    year = car.Year,
                    driverName = car.Driver?.Name ?? "Unassigned",
                    detailsUrl = Url.Action(nameof(Details), new { id = car.Id }),
                    editUrl = Url.Action(nameof(Edit), new { id = car.Id }),
                    deleteUrl = Url.Action(nameof(Delete), new { id = car.Id })
                });

            return Json(cars);
        }

        private void PopulateCarOptions(CarFormViewModel viewModel)
        {
            viewModel.TireOptions = _carRepository.GetTires()
                .Select(tire => new SelectListItem(
                    $"{tire.Brand} {tire.Model} - {tire.Type}",
                    tire.Id.ToString()))
                .ToList();

            viewModel.SuspensionOptions = _carRepository.GetSuspensions()
                .Select(suspension => new SelectListItem(
                    $"{suspension.Brand} {suspension.Type}",
                    suspension.Id.ToString()))
                .ToList();

            viewModel.RimOptions = _tireRepository.GetRims()
                .Select(rim => new SelectListItem(
                    $"{rim.Make} {rim.Model} - {rim.SizeInJ} J",
                    rim.Id.ToString()))
                .ToList();
        }

        private void ValidateCarReferences(CarFormViewModel viewModel)
        {
            if (viewModel.DriverId.HasValue && _driverRepository.GetById(viewModel.DriverId.Value) == null)
            {
                ModelState.AddModelError(nameof(CarFormViewModel.DriverId), "Select an existing driver.");
            }

            if (viewModel.TireId.HasValue && _carRepository.GetTireById(viewModel.TireId.Value) == null)
            {
                ModelState.AddModelError(nameof(CarFormViewModel.TireId), "Select an existing tire.");
            }

            if (viewModel.SuspensionId.HasValue && _carRepository.GetSuspensionById(viewModel.SuspensionId.Value) == null)
            {
                ModelState.AddModelError(nameof(CarFormViewModel.SuspensionId), "Select an existing suspension.");
            }
        }

        private void PopulateDriverName(CarFormViewModel viewModel)
        {
            if (viewModel.DriverId.HasValue)
            {
                viewModel.DriverName = _driverRepository.GetById(viewModel.DriverId.Value)?.Name ?? string.Empty;
            }
        }
    }
}
