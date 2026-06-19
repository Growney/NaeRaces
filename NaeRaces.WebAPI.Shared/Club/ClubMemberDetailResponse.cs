namespace NaeRaces.WebAPI.Shared.Club;

public class ClubMemberDetailResponse
{
    public Guid PilotId { get; set; }
    public string CallSign { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Nationality { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? MembershipLevelId { get; set; }
    public string? MembershipLevelName { get; set; }
    public int? PaymentOptionId { get; set; }
    public string? PaymentOptionName { get; set; }
    public DateTime? ValidUntil { get; set; }
}
