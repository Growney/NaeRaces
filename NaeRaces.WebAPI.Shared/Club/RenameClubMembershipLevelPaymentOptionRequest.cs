using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class RenameClubMembershipLevelPaymentOptionRequest
{
    [Required]
    public string NewName { get; set; } = string.Empty;
}
