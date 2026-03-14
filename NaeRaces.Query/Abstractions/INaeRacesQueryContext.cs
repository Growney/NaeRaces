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
    IRaceInformationQueryHandler RaceInformation { get; }
    IRaceDiscountQueryHandler RaceCost { get; }
    IRacePackageQueryHandler RacePackage { get; }
    IPilotSelectionPolicyQueryHandler PilotSelectionPolicy { get; }
    ITeamMemberQueryHandler TeamMember { get; }
    IPilotFollowedClubQueryHandler PilotFollowedClub { get; }
}
