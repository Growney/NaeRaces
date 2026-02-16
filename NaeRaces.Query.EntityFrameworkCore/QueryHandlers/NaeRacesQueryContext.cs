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
        IPilotRegistrationDetailsQueryHandler pilotRegistrationDetails,
        IRaceDetailsQueryHandler raceDetails,
        IRaceCostQueryHandler raceCost,
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
        PilotRegistrationDetails = pilotRegistrationDetails;
        RaceDetails = raceDetails;
        RaceCost = raceCost;
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
    public IPilotRegistrationDetailsQueryHandler PilotRegistrationDetails { get; }
    public IRaceDetailsQueryHandler RaceDetails { get; }
    public IRaceCostQueryHandler RaceCost { get; }
    public IRaceRegistrationDatesQueryHandler RaceRegistrationDates { get; }
    public IPilotSelectionPolicyQueryHandler PilotSelectionPolicy { get; }
    public ITeamMemberQueryHandler TeamMember { get; }
}
