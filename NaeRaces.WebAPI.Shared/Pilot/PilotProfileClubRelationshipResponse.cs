namespace NaeRaces.WebAPI.Shared.Pilot;

public class PilotProfileClubRelationshipResponse
{
    public PilotProfileClubMembershipResponse? Membership { get; set; }
    public bool IsFollowing { get; set; }
    public List<string> Roles { get; set; } = [];
}
