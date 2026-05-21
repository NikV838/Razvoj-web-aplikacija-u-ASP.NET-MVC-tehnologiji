using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels
{
    public class SuspensionCreateAjaxViewModel
    {
        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(72, MinimumLength = 2)]
        public string Brand { get; set; } = string.Empty;

        [Range(40, 240)]
        public double RideHeightMm { get; set; }

        public bool HasFrontStrutBar { get; set; }

        public bool HasRearStrutBar { get; set; }

        public bool IsHeightAdjustable { get; set; }

        public bool IsStiffnessAdjustable { get; set; }

        [Range(0, 20)]
        public double? FrontStiffness { get; set; }

        [Range(0, 20)]
        public double? RearStiffness { get; set; }
    }
}
