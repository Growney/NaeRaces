namespace NaeRaces.Query.Abstractions;

public interface INaeRacesQueryContext
{
    IClubDetailsQueryHandler ClubDetails { get; }
    IClubLocationQueryHandler ClubLocation { get; }
    IClubMemberQueryHandler ClubMember { get; }
    IClubMembershipLevelQueryHandler ClubMembershipLevel { get; }
    IClubUniquenessQueryHandler ClubUniqueness { get; }
    IPilotDetailsQueryHandler PilotDetails { get; }
    IPilotValidationQueryHandler PilotValidation { get; }
    IPilotRegistrationDetailsQueryHandler PilotRegistrationDetails { get; }
    IRaceDetailsQueryHandler RaceDetails { get; }
    IPilotSelectionPolicyQueryHandler PilotSelectionPolicy { get; }
    ITeamMemberQueryHandler TeamMember { get; }
}
