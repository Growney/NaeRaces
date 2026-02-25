using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class AddClubMembershipLevelRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
