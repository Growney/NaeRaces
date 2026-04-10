namespace NaeRaces.WebAPI.Shared.Club;

public class ClubMembershipLevelResponse
{
    public int MembershipLevelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? PilotPolicyId { get; set; }
    public string? PilotPolicyName { get; set; }
    public List<ClubMembershipLevelPaymentOptionResponse> PaymentOptions { get; set; } = [];
}
