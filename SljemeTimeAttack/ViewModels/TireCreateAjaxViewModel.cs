using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels
{
    public class TireCreateAjaxViewModel
    {
        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(72, MinimumLength = 2)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string Type { get; set; } = string.Empty;

        [Range(100, 400)]
        public double SizeInMm { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 3)]
        public string Dot { get; set; } = string.Empty;

        [Required]
        public int? RimId { get; set; }
    }
}
