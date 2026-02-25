using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class SetClubMembershipLevelPolicyRequest
{
    [Required]
    public Guid PilotSelectionPolicyId { get; set; }
}
