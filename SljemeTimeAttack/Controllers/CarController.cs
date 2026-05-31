using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;

        public CarController(
            CarEfRepository carRepository,
            DriverEfRepository driverRepository,
            TireEfRepository tireRepository,
            UserManager<AppUser> userManager)
        {
            _carRepository = carRepository;
            _driverRepository = driverRepository;
            _tireRepository = tireRepository;
            _userManager = userManager;
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

        [Authorize(Roles = "Admin,User,Racer")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CarCreateViewModel();
            await ApplyDriverOwnershipDefaults(viewModel);
            PopulateCarOptions(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User,Racer")]
        public async Task<IActionResult> Create(CarCreateViewModel viewModel)
        {
            await ApplyDriverOwnershipDefaults(viewModel);
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

        [Authorize(Roles = "Admin,User,Racer")]
        public async Task<IActionResult> Edit(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();
            if (!await CanManageCar(car)) return Forbid();

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
                SuspensionId = car.SuspensionId,
                CanChooseDriver = User.IsInRole("Admin")
            };
            PopulateCarOptions(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User,Racer")]
        public async Task<IActionResult> Edit(int id, CarEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();
            var existingCar = _carRepository.GetById(id);
            if (existingCar == null) return NotFound();
            if (!await CanManageCar(existingCar)) return Forbid();

            await ApplyDriverOwnershipDefaults(viewModel);

            ValidateCarReferences(viewModel);

            if (!ModelState.IsValid)
            {
                PopulateCarOptions(viewModel);
                PopulateDriverName(viewModel);
                return View(viewModel);
            }

            existingCar.Make = viewModel.Make;
            existingCar.Model = viewModel.Model;
            existingCar.Horsepower = viewModel.Horsepower;
            existingCar.WeightKg = viewModel.WeightKg;
            existingCar.Year = viewModel.Year;
            existingCar.RegistrationNumber = viewModel.RegistrationNumber;
            existingCar.DriverId = viewModel.DriverId;
            existingCar.TireId = viewModel.TireId!.Value;
            existingCar.SuspensionId = viewModel.SuspensionId!.Value;

            _carRepository.Update(existingCar);
            return RedirectToAction(nameof(Details), new { id = existingCar.Id });
        }

        [Authorize(Roles = "Admin,User,Racer")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();
            if (!await CanManageCar(car)) return Forbid();

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
        [Authorize(Roles = "Admin,User,Racer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();
            if (!await CanManageCar(car)) return Forbid();

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

        private async Task ApplyDriverOwnershipDefaults(CarFormViewModel viewModel)
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
            viewModel.DriverName = driver.Name;
        }

        private async Task<bool> CanManageCar(Car car)
        {
            if (User.IsInRole("Admin")) return true;
            var driver = await GetCurrentDriverProfile();
            return driver != null && car.DriverId == driver.Id;
        }

        private async Task<Driver?> GetCurrentDriverProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            return user == null
                ? null
                : _driverRepository.GetAll().FirstOrDefault(driver => driver.AppUserId == user.Id);
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
