using NaeRaces.Query.Abstractions;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class NaeRacesQueryContext : INaeRacesQueryContext
{
    public NaeRacesQueryContext(
        IClubDetailsQueryHandler clubDetails,
        IClubLocationQueryHandler clubLocation,
        IClubMemberQueryHandler clubMember,
        IClubUniquenessQueryHandler clubUniqueness,
        IPilotDetailsQueryHandler pilotDetails,
        IPilotValidationQueryHandler pilotValidation,
        IPilotRegistrationDetailsQueryHandler pilotRegistrationDetails,
        IRaceDetailsQueryHandler raceDetails,
        IRacePolicyQueryHandler racePolicy,
        ITeamMemberQueryHandler teamMember)
    {
        ClubDetails = clubDetails;
        ClubLocation = clubLocation;
        ClubMember = clubMember;
        ClubUniqueness = clubUniqueness;
        PilotDetails = pilotDetails;
        PilotValidation = pilotValidation;
        PilotRegistrationDetails = pilotRegistrationDetails;
        RaceDetails = raceDetails;
        RacePolicy = racePolicy;
        TeamMember = teamMember;
    }

    public IClubDetailsQueryHandler ClubDetails { get; }
    public IClubLocationQueryHandler ClubLocation { get; }
    public IClubMemberQueryHandler ClubMember { get; }
    public IClubUniquenessQueryHandler ClubUniqueness { get; }
    public IPilotDetailsQueryHandler PilotDetails { get; }
    public IPilotValidationQueryHandler PilotValidation { get; }
    public IPilotRegistrationDetailsQueryHandler PilotRegistrationDetails { get; }
    public IRaceDetailsQueryHandler RaceDetails { get; }
    public IRacePolicyQueryHandler RacePolicy { get; }
    public ITeamMemberQueryHandler TeamMember { get; }
}
