using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class ManuallyConfirmPilotClubMembershipRequest
{
    [Required]
    public Guid RegistrationId { get; set; }

    [Required]
    public Guid ConfirmedByPilotId { get; set; }

    [Required]
    public DateTime ValidUntil { get; set; }
}
