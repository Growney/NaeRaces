using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.PilotSelectionPolicy;

public class AddMaximumAgeRequirementRequest
{
    [Required]
    public int MaximumAge { get; set; }

    [Required]
    public string ValidationPolicy { get; set; } = string.Empty;
}
