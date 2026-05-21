using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels
{
    public class RunEditViewModel : RunFormViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
