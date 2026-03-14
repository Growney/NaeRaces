using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class AssignClubMemberRoleRequest
{
    [Required]
    public Guid PilotId { get; set; }

    [Required]
    public ClubMemberRole Role { get; set; }
}
