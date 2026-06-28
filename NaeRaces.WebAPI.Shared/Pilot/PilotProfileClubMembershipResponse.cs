namespace NaeRaces.WebAPI.Shared.Pilot;

public class PilotProfileClubMembershipResponse
{
    public int MembershipLevelId { get; set; }
    public string MembershipLevelName { get; set; } = string.Empty;
    public int PaymentOptionId { get; set; }
    public string PaymentOptionName { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public DateTime? ValidUntil { get; set; }
}
