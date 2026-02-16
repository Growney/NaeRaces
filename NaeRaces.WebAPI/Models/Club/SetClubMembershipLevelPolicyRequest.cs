using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class SetClubMembershipLevelPolicyRequest
{
    [Required]
    public Guid PilotSelectionPolicyId { get; set; }
}
