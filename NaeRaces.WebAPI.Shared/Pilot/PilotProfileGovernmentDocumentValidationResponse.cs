namespace NaeRaces.WebAPI.Shared.Pilot;

public class PilotProfileGovernmentDocumentValidationResponse
{
    public string GovernmentDocument { get; set; } = string.Empty;
    public DateTime ValidUntil { get; set; }
    public Guid ClubId { get; set; }
    public Guid ValidatedByPilotId { get; set; }
    public bool IsValidatingMemberOnCommiteeOfClub { get; set; }
}
