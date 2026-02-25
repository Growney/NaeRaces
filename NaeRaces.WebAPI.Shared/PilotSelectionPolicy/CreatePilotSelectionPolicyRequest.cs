using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

public class CreatePilotSelectionPolicyRequest
{
    [Required]
    public Guid PilotSelectionPolicyId { get; set; }

    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}
