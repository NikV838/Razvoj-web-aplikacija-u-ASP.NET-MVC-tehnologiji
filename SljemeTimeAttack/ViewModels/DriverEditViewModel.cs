using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels
{
    public class DriverEditViewModel : DriverFormViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
