using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;
using SljemeTimeAttack.Services;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class CarController : Controller
    {
        private readonly CarEfRepository _carRepository;
        private readonly DriverEfRepository _driverRepository;
        private readonly TireEfRepository _tireRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CarController> _logger;
        private readonly IAiCarParserService _aiCarParserService;
        private readonly IGarageDeletionService _garageDeletionService;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif"
        };

        public CarController(
            CarEfRepository carRepository,
            DriverEfRepository driverRepository,
            TireEfRepository tireRepository,
            UserManager<AppUser> userManager,
            IWebHostEnvironment environment,
            ILogger<CarController> logger,
            IAiCarParserService aiCarParserService,
            IGarageDeletionService garageDeletionService)
        {
            _carRepository = carRepository;
            _driverRepository = driverRepository;
            _tireRepository = tireRepository;
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
            _aiCarParserService = aiCarParserService;
            _garageDeletionService = garageDeletionService;
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

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CarCreateViewModel();
            await ApplyDriverOwnershipDefaults(viewModel);
            PopulateCarOptions(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ParseCarPrompt([FromBody] CarPromptParseRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Enter a car description first." });
            }

            var parsedCar = await _aiCarParserService.ParseAsync(request.Prompt, cancellationToken);
            if (!parsedCar.HasAnyField)
            {
                return UnprocessableEntity(new { error = "No car fields could be detected. Try adding make, model, year, horsepower, weight, or registration." });
            }

            _logger.LogInformation("Car prompt parsed for form fill. Source: {Source}, User: {UserName}", parsedCar.Source, User.Identity?.Name);
            return Json(parsedCar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CarCreateViewModel viewModel)
        {
            await ApplyDriverOwnershipDefaults(viewModel);
            ValidateCarReferences(viewModel);
            ValidateImageFile(viewModel);

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
                ImagePath = await SaveCarImageAsync(viewModel.ImageFile),
                DriverId = viewModel.DriverId,
                TireId = viewModel.TireId!.Value,
                SuspensionId = viewModel.SuspensionId!.Value
            };

            _carRepository.Add(car);
            _logger.LogInformation("Car created. CarId: {CarId}, Car: {Make} {Model}, DriverId: {DriverId}, User: {UserName}", car.Id, car.Make, car.Model, car.DriverId, User.Identity?.Name);
            if (!string.IsNullOrWhiteSpace(car.ImagePath))
            {
                _logger.LogInformation("File upload completed for car image. CarId: {CarId}, ImagePath: {ImagePath}", car.Id, car.ImagePath);
            }
            return RedirectToAction(nameof(Details), new { id = car.Id });
        }

        [Authorize]
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
                ExistingImagePath = car.ImagePath,
                CanChooseDriver = User.IsInRole("Admin")
            };
            PopulateCarOptions(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, CarEditViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();
            var existingCar = _carRepository.GetById(id);
            if (existingCar == null) return NotFound();
            if (!await CanManageCar(existingCar)) return Forbid();

            await ApplyDriverOwnershipDefaults(viewModel);

            ValidateCarReferences(viewModel);
            ValidateImageFile(viewModel);

            if (!ModelState.IsValid)
            {
                PopulateCarOptions(viewModel);
                PopulateDriverName(viewModel);
                viewModel.ExistingImagePath = existingCar.ImagePath;
                return View(viewModel);
            }

            existingCar.Make = viewModel.Make;
            existingCar.Model = viewModel.Model;
            existingCar.Horsepower = viewModel.Horsepower;
            existingCar.WeightKg = viewModel.WeightKg;
            existingCar.Year = viewModel.Year;
            existingCar.RegistrationNumber = viewModel.RegistrationNumber;
            if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
            {
                var oldImagePath = existingCar.ImagePath;
                existingCar.ImagePath = await SaveCarImageAsync(viewModel.ImageFile);
                DeleteCarImage(oldImagePath);
                _logger.LogInformation("File upload completed for car image update. CarId: {CarId}, ImagePath: {ImagePath}", existingCar.Id, existingCar.ImagePath);
            }
            existingCar.DriverId = viewModel.DriverId;
            existingCar.TireId = viewModel.TireId!.Value;
            existingCar.SuspensionId = viewModel.SuspensionId!.Value;

            _carRepository.Update(existingCar);
            _logger.LogInformation("Car updated. CarId: {CarId}, Car: {Make} {Model}, DriverId: {DriverId}, User: {UserName}", existingCar.Id, existingCar.Make, existingCar.Model, existingCar.DriverId, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id = existingCar.Id });
        }

        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = _carRepository.GetById(id);
            if (car == null) return NotFound();
            if (!await CanManageCar(car)) return Forbid();

            await _garageDeletionService.DeleteCarAsync(car, User.Identity?.Name);
            _logger.LogInformation("Car deleted. CarId: {CarId}, Car: {Make} {Model}, User: {UserName}", car.Id, car.Make, car.Model, User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search(string? query)
        {
            var isAuthenticated = User.Identity?.IsAuthenticated == true;
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
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
                    deleteUrl = Url.Action(nameof(Delete), new { id = car.Id }),
                    imagePath = GetCarListImagePath(car),
                    canManage = isAdmin || (isAuthenticated && car.Driver?.AppUserId == currentUserId)
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

            viewModel.RimChoices = _tireRepository.GetRims();
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

        private void ValidateImageFile(CarFormViewModel viewModel)
        {
            if (viewModel.ImageFile == null || viewModel.ImageFile.Length == 0)
            {
                return;
            }

            var extension = Path.GetExtension(viewModel.ImageFile.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(CarFormViewModel.ImageFile), "Upload a JPG, PNG, WEBP, or GIF image.");
            }
        }

        private async Task<string?> SaveCarImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var uploadRoot = Path.Combine(_environment.WebRootPath, "img", "cars");
            Directory.CreateDirectory(uploadRoot);
            var physicalPath = Path.Combine(uploadRoot, fileName);

            await using var stream = System.IO.File.Create(physicalPath);
            await file.CopyToAsync(stream);

            return $"/img/cars/{fileName}";
        }

        private void DeleteCarImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !imagePath.StartsWith("/img/cars/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileName = Path.GetFileName(imagePath);
            var physicalPath = Path.Combine(_environment.WebRootPath, "img", "cars", fileName);
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        private void PopulateDriverName(CarFormViewModel viewModel)
        {
            if (viewModel.DriverId.HasValue)
            {
                viewModel.DriverName = _driverRepository.GetById(viewModel.DriverId.Value)?.Name ?? string.Empty;
            }
        }

        private static string? GetCarListImagePath(Car car)
        {
            if (!string.IsNullOrWhiteSpace(car.ImagePath))
            {
                return car.ImagePath;
            }

            return car.Id switch
            {
                1 => "/img/Mazda-RX8.avif",
                2 => "/img/toyota-celica.jpg",
                3 => "/img/toyota-mr2.webp",
                4 => "/img/honda-eg6.jpg",
                5 => "/img/honda-s2k.webp",
                _ => null
            };
        }
    }
}
