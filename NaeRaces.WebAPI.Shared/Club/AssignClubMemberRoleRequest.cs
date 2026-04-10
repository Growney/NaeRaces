using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class AssignClubMemberRoleRequest
{
    [Required]
    public Guid PilotId { get; set; }

    [Required]
    public required string Role { get; set; }
}
