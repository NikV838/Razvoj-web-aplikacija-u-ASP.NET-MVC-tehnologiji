using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;
using SljemeTimeAttack.ViewModels;

namespace SljemeTimeAttack.Controllers
{
    public class RimController : Controller
    {
        private readonly SljemeTimeAttackDbContext _context;

        public RimController(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAjax(RimCreateAjaxViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = GetValidationErrors() });
            }

            var rim = new Rim
            {
                Make = viewModel.Make,
                Model = viewModel.Model,
                SizeInJ = viewModel.SizeInJ,
                Material = viewModel.Material
            };

            _context.Rims.Add(rim);
            _context.SaveChanges();

            return Json(new
            {
                id = rim.Id,
                text = $"{rim.Model} - {rim.SizeInJ} J {rim.Material}",
                make = rim.Make,
                model = rim.Model,
                sizeInJ = rim.SizeInJ,
                material = rim.Material
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
