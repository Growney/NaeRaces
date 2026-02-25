using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

public class AddInsuranceProviderRequirementRequest
{
    [Required]
    public string InsuranceProvider { get; set; } = string.Empty;

    [Required]
    public string ValidationPolicy { get; set; } = string.Empty;
}
