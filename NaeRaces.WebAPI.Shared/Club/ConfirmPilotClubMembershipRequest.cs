using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class ConfirmPilotClubMembershipRequest
{
    [Required]
    public Guid RegistrationId { get; set; }

    [Required]
    public DateTime ValidUntil { get; set; }
}
