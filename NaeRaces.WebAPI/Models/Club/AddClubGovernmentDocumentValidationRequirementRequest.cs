using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class AddClubGovernmentDocumentValidationRequirementRequest
{
    [Required]
    public string GovernmentDocument { get; set; } = string.Empty;

    [Required]
    public string ValidationPolicy { get; set; } = string.Empty;
}
