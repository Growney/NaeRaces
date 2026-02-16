using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.PilotSelectionPolicy;

public class AddMinimumAgeRequirementRequest
{
    [Required]
    public int MinimumAge { get; set; }

    [Required]
    public string ValidationPolicy { get; set; } = string.Empty;
}
