namespace NaeRaces.WebAPI.Shared.Club;

public class MyClubMembershipResponse
{
    public Guid ClubId { get; set; }
    public string ClubCode { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public string? MembershipLevelName { get; set; }
    public DateTime? MembershipExpiry { get; set; }
    public bool IsFollowing { get; set; }
    public List<string> Roles { get; set; } = [];
}
