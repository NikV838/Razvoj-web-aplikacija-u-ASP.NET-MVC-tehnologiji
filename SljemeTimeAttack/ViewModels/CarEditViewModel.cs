using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels
{
    public class CarEditViewModel : CarFormViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
