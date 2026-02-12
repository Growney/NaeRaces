using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class AddClubMembershipLevelRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
