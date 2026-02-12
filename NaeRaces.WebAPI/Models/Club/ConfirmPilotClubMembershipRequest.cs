using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class ConfirmPilotClubMembershipRequest
{
    [Required]
    public int MembershipLevelId { get; set; }

    [Required]
    public int PaymentOptionId { get; set; }

    [Required]
    public Guid RegistrationId { get; set; }

    [Required]
    public DateTime ValidUntil { get; set; }
}
