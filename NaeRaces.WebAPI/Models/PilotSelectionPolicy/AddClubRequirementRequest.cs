using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.PilotSelectionPolicy;

public class AddClubRequirementRequest
{
    [Required]
    public Guid ClubId { get; set; }
}
