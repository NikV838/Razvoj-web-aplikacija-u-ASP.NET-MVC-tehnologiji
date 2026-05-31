using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.Repos;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class SuspensionController : Controller
    {
        private readonly SuspensionEfRepository _suspensionRepository;

        public SuspensionController(SuspensionEfRepository suspensionRepository)
        {
            _suspensionRepository = suspensionRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User,Racer")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAjax(SuspensionCreateAjaxViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = GetValidationErrors() });
            }

            var suspension = new Suspension
            {
                Type = viewModel.Type,
                Brand = viewModel.Brand,
                RideHeightMm = viewModel.RideHeightMm,
                HasFrontStrutBar = viewModel.HasFrontStrutBar,
                HasRearStrutBar = viewModel.HasRearStrutBar,
                IsHeightAdjustable = viewModel.IsHeightAdjustable,
                IsStiffnessAdjustable = viewModel.IsStiffnessAdjustable,
                FrontStiffness = viewModel.FrontStiffness,
                RearStiffness = viewModel.RearStiffness
            };

            _suspensionRepository.Add(suspension);

            return Json(new
            {
                id = suspension.Id,
                text = $"{suspension.Brand} {suspension.Type}"
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
