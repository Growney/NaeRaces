using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class RenameClubMembershipLevelPaymentOptionRequest
{
    [Required]
    public string NewName { get; set; } = string.Empty;
}
