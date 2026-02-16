using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class SetClubMembershipLevelPolicyRequest
{
    [Required]
    public Guid RacePolicyId { get; set; }
}
