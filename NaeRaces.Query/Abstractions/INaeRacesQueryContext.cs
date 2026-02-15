namespace NaeRaces.Query.Abstractions;

public interface INaeRacesQueryContext
{
    IClubDetailsQueryHandler ClubDetails { get; }
    IClubLocationQueryHandler ClubLocation { get; }
    IClubMemberQueryHandler ClubMember { get; }
    IClubUniquenessQueryHandler ClubUniqueness { get; }
    IPilotDetailsQueryHandler PilotDetails { get; }
    IPilotValidationQueryHandler PilotValidation { get; }
    IPilotRegistrationDetailsQueryHandler PilotRegistrationDetails { get; }
    IRaceDetailsQueryHandler RaceDetails { get; }
    IRacePolicyQueryHandler RacePolicy { get; }
    ITeamMemberQueryHandler TeamMember { get; }
}
