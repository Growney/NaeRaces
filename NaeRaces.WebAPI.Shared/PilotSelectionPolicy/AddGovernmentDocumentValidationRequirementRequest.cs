using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

public class AddGovernmentDocumentValidationRequirementRequest
{
    [Required]
    public string GovernmentDocument { get; set; } = string.Empty;

    [Required]
    public string ValidationPolicy { get; set; } = string.Empty;
}
