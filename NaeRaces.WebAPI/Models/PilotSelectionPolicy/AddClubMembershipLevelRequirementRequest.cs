using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.PilotSelectionPolicy;

public class AddClubMembershipLevelRequirementRequest
{
    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public int MembershipLevel { get; set; }
}
