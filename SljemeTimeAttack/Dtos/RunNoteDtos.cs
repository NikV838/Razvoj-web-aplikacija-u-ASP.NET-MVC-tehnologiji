using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Dtos;

public record RunNoteDto(int Id, int RunId, RunDto? Run, string Note, DateTime CreatedDate);

public class RunNoteUpsertDto
{
    [Range(1, int.MaxValue)]
    public int RunId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Note { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; }
}
