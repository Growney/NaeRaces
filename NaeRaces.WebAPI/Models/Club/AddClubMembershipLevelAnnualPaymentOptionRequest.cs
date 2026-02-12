using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class AddClubMembershipLevelAnnualPaymentOptionRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Currency { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }
}
