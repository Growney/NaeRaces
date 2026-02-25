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
    IPilotPolicyValidationQueryHandler PilotPolicyValidation { get; }
    IPilotRegistrationQueryHandler PilotRegistration { get; }
    IRaceDetailsQueryHandler RaceDetails { get; }
    IRaceDiscountQueryHandler RaceCost { get; }
    IRacePackageQueryHandler RacePackage { get; }
    IRaceRegistrationDatesQueryHandler RaceRegistrationDates { get; }
    IPilotSelectionPolicyQueryHandler PilotSelectionPolicy { get; }
    ITeamMemberQueryHandler TeamMember { get; }
}
