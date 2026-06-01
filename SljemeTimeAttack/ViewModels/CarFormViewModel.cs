using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SljemeTimeAttack.ViewModels
{
    public abstract class CarFormViewModel
    {
        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [StringLength(72, MinimumLength = 1)]
        public string Model { get; set; } = string.Empty;

        [Range(40, 2000)]
        public int Horsepower { get; set; }

        [Range(400, 4000)]
        public double WeightKg { get; set; }

        [Range(1950, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 3)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        public int? DriverId { get; set; }

        [Required]
        public int? TireId { get; set; }

        [Required]
        public int? SuspensionId { get; set; }

        public string DriverName { get; set; } = string.Empty;

        public bool CanChooseDriver { get; set; } = true;

        public IFormFile? ImageFile { get; set; }

        public string? ExistingImagePath { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> TireOptions { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> SuspensionOptions { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> RimOptions { get; set; } = new List<SelectListItem>();
    }
}
