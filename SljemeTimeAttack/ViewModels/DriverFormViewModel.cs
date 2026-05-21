using System.ComponentModel.DataAnnotations;
namespace SljemeTimeAttack.ViewModels
{
    public abstract class DriverFormViewModel
    {
        [Required]
        [StringLength(32, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(16, 100)]
        public int Age { get; set; }

        [Range(0, 80)]
        public int YearsOfExperience { get; set; }

        [Required]
        public int? TeamId { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string TeamName { get; set; } = string.Empty;
    }
}
