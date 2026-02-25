using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

public class AddClubRequirementRequest
{
    [Required]
    public Guid ClubId { get; set; }
}
