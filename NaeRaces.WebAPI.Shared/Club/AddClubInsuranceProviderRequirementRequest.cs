using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class AddClubInsuranceProviderRequirementRequest
{
    [Required]
    public string InsuranceProvider { get; set; } = string.Empty;

    [Required]
    public string ValidationPolicy { get; set; } = string.Empty;
}
