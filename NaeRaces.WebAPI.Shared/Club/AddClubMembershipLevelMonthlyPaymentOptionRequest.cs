using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class AddClubMembershipLevelMonthlyPaymentOptionRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int DayOfMonthDue { get; set; }

    [Required]
    public int PaymentInterval { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }
}
