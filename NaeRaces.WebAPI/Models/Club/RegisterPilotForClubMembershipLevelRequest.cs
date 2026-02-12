using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class RegisterPilotForClubMembershipLevelRequest
{
    [Required]
    public int MembershipLevelId { get; set; }

    [Required]
    public int PaymentOptionId { get; set; }

    [Required]
    public Guid PilotId { get; set; }

    [Required]
    public Guid RegistrationId { get; set; }
}
