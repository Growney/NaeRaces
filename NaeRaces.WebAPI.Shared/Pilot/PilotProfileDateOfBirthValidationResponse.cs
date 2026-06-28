namespace NaeRaces.WebAPI.Shared.Pilot;

public class PilotProfileDateOfBirthValidationResponse
{
    public Guid ClubId { get; set; }
    public Guid ValidatedByPilotId { get; set; }
    public bool IsValidatingMemberOnCommiteeOfClub { get; set; }
}
