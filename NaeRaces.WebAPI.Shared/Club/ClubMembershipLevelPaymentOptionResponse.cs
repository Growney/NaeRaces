namespace NaeRaces.WebAPI.Shared.Club;

public class ClubMembershipLevelPaymentOptionResponse
{
    public int PaymentOptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int? DayOfMonthDue { get; set; }
    public int? PaymentInterval { get; set; }
}
