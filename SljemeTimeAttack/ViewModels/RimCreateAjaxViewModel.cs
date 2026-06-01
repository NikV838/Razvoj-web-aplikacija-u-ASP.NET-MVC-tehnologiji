using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels
{
    public class RimCreateAjaxViewModel
    {
        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [StringLength(72, MinimumLength = 2)]
        public string Model { get; set; } = string.Empty;

        [Range(4, 14)]
        public double SizeInJ { get; set; }

        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string Material { get; set; } = string.Empty;
    }
}
