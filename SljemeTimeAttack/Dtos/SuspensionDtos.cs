using System.ComponentModel.DataAnnotations;

namespace SljemeTimeAttack.Dtos;

public record SuspensionDto(
    int Id,
    string Type,
    string Brand,
    bool HasFrontStrutBar,
    bool HasRearStrutBar,
    double RideHeightMm,
    bool IsHeightAdjustable,
    bool IsStiffnessAdjustable,
    double? FrontStiffness,
    double? RearStiffness);

public class SuspensionUpsertDto
{
    [Required]
    [StringLength(80)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Brand { get; set; } = string.Empty;

    public bool HasFrontStrutBar { get; set; }

    public bool HasRearStrutBar { get; set; }

    [Range(30, 250)]
    public double RideHeightMm { get; set; }

    public bool IsHeightAdjustable { get; set; }

    public bool IsStiffnessAdjustable { get; set; }

    [Range(0, 50)]
    public double? FrontStiffness { get; set; }

    [Range(0, 50)]
    public double? RearStiffness { get; set; }
}
