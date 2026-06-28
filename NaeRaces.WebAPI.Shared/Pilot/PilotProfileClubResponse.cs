namespace NaeRaces.WebAPI.Shared.Pilot;

public class PilotProfileClubResponse
{
    public Guid ClubId { get; set; }
    public string ClubCode { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public PilotProfileClubRelationshipResponse Relationship { get; set; } = new();
}
