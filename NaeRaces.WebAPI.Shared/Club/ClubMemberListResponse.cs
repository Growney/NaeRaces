namespace NaeRaces.WebAPI.Shared.Club;

public class ClubMemberListResponse
{
    public Guid PilotId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? MembershipLevelName { get; set; }
    public DateTime? MembershipExpiry { get; set; }
    public bool IsRegistrationConfirmed { get; set; }
    public Guid? RegistrationId { get; set; }
    public int? MembershipLevelId { get; set; }
    public int? PaymentOptionId { get; set; }
}
