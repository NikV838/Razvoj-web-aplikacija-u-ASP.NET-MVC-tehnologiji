using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.ViewModels
{
    public class CarPromptParseRequest
    {
        [Required]
        public string Prompt { get; set; } = string.Empty;
    }
}
