using NaeRaces.Query.Abstractions;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class NaeRacesQueryContext : INaeRacesQueryContext
{
    public NaeRacesQueryContext(
        IClubDetailsQueryHandler clubDetails,
        IClubLocationQueryHandler clubLocation,
        IClubMemberQueryHandler clubMember,
        IClubMembershipLevelQueryHandler clubMembershipLevel,
        IClubUniquenessQueryHandler clubUniqueness,
        IPilotDetailsQueryHandler pilotDetails,
        IPilotValidationQueryHandler pilotValidation,
        IPilotPolicyValidationQueryHandler pilotPolicyValidation,
        IPilotRegistrationQueryHandler pilotRegistrationDetails,
        IRaceDetailsQueryHandler raceDetails,
        IRaceDiscountQueryHandler raceCost,
        IRacePackageQueryHandler racePackage,
        IRaceRegistrationDatesQueryHandler raceRegistrationDates,
        IPilotSelectionPolicyQueryHandler pilotSelectionPolicy,
        ITeamMemberQueryHandler teamMember)
    {
        ClubDetails = clubDetails;
        ClubLocation = clubLocation;
        ClubMember = clubMember;
        ClubMembershipLevel = clubMembershipLevel;
        ClubUniqueness = clubUniqueness;
        PilotDetails = pilotDetails;
        PilotValidation = pilotValidation;
        PilotPolicyValidation = pilotPolicyValidation;
        PilotRegistration = pilotRegistrationDetails;
        RaceDetails = raceDetails;
        RaceCost = raceCost;
        RacePackage = racePackage;
        RaceRegistrationDates = raceRegistrationDates;
        PilotSelectionPolicy = pilotSelectionPolicy;
        TeamMember = teamMember;
    }

    public IClubDetailsQueryHandler ClubDetails { get; }
    public IClubLocationQueryHandler ClubLocation { get; }
    public IClubMemberQueryHandler ClubMember { get; }
    public IClubMembershipLevelQueryHandler ClubMembershipLevel { get; }
    public IClubUniquenessQueryHandler ClubUniqueness { get; }
    public IPilotDetailsQueryHandler PilotDetails { get; }
    public IPilotValidationQueryHandler PilotValidation { get; }
    public IPilotPolicyValidationQueryHandler PilotPolicyValidation { get; }
    public IPilotRegistrationQueryHandler PilotRegistration { get; }
    public IRaceDetailsQueryHandler RaceDetails { get; }
    public IRaceDiscountQueryHandler RaceCost { get; }
    public IRacePackageQueryHandler RacePackage { get; }
    public IRaceRegistrationDatesQueryHandler RaceRegistrationDates { get; }
    public IPilotSelectionPolicyQueryHandler PilotSelectionPolicy { get; }
    public ITeamMemberQueryHandler TeamMember { get; }
}
