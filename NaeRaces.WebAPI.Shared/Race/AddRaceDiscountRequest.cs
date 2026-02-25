using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class AddRaceDiscountRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Guid PilotPolicyId { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [Range(0, 1)]
    public decimal Discount { get; set; }

    public bool CanBeCombined { get; set; } = false;

    public bool IsPercentage { get; set; } = false;
}
