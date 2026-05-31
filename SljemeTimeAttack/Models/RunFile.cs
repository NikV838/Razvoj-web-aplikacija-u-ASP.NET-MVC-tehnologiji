using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SljemeTimeAttack.Models;

public class RunFile
{
    [Key]
    public int Id { get; set; }

    public int RunId { get; set; }

    [ForeignKey(nameof(RunId))]
    public Run Run { get; set; } = null!;

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }
}
