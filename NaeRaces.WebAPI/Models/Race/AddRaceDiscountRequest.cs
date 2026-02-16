using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class AddRaceDiscountRequest
{
    [Required]
    public Guid PilotPolicyId { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [Range(0, 1)]
    public decimal Discount { get; set; }
}
