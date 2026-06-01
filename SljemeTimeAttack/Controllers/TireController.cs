using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class TireController : Controller
    {
        private readonly TireEfRepository _tireRepository;

        public TireController(TireEfRepository tireRepository)
        {
            _tireRepository = tireRepository;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAjax(TireCreateAjaxViewModel viewModel)
        {
            if (viewModel.RimId.HasValue && _tireRepository.GetRimById(viewModel.RimId.Value) == null)
            {
                ModelState.AddModelError(nameof(TireCreateAjaxViewModel.RimId), "Select an existing rim.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = GetValidationErrors() });
            }

            var tire = new Tire
            {
                Brand = viewModel.Brand,
                Model = viewModel.Model,
                Type = viewModel.Type,
                SizeInMm = viewModel.SizeInMm,
                Dot = viewModel.Dot,
                RimId = viewModel.RimId!.Value
            };

            _tireRepository.Add(tire);

            return Json(new
            {
                id = tire.Id,
                text = $"{tire.Brand} {tire.Model} - {tire.Type}"
            });
        }

        private Dictionary<string, string[]> GetValidationErrors()
        {
            return ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());
        }
    }
}
