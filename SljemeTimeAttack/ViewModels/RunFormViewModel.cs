using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SljemeTimeAttack.Enums;

namespace SljemeTimeAttack.ViewModels
{
    public abstract class RunFormViewModel
    {
        [Required]
        public int? DriverId { get; set; }

        [Required]
        public int? CarId { get; set; }

        [Required]
        public Track? Track { get; set; }

        [Required]
        [RegularExpression(@"^\d{1,2}:\d{2}$", ErrorMessage = "Use minutes:seconds, for example 5:20.")]
        public string BestTime { get; set; } = string.Empty;

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public DriveDirection? Direction { get; set; }

        [Required]
        public WeatherCondition? Weather { get; set; }

        public bool CanChooseDriver { get; set; } = true;

        [ValidateNever]
        public IEnumerable<SelectListItem> DriverOptions { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> CarOptions { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> TrackOptions { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> DirectionOptions { get; set; } = new List<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> WeatherOptions { get; set; } = new List<SelectListItem>();
    }
}
