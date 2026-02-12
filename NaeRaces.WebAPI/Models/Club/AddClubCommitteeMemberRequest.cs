using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class AddClubCommitteeMemberRequest
{
    [Required]
    public Guid PilotId { get; set; }

    [Required]
    public string Role { get; set; } = string.Empty;
}
