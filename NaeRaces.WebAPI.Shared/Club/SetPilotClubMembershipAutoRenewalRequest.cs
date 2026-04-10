using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class SetPilotClubMembershipAutoRenewalRequest
{
    [Required]
    public Guid RegistrationId { get; set; }

    [Required]
    public bool AutoRenew { get; set; }
}
