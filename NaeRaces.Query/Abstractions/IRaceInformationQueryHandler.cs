using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IRaceInformationQueryHandler
{
    Task<RaceInformation?> GetRaceInformation(Guid raceId);
    IAsyncEnumerable<RaceInformation> GetRaceInformationAfterDate(DateTime afterDate);
    IAsyncEnumerable<RaceInformation> GetRaceInformationForClub(Guid clubId);
    IAsyncEnumerable<RaceInformation> GetRaceInformationForClubs(IEnumerable<Guid> clubIds);
    IAsyncEnumerable<RaceInformation> GetRaceInformationForClubsAfterDate(IEnumerable<Guid> clubIds, DateTime afterDate);
    IAsyncEnumerable<RaceInformation> GetRaceInformationForPilot(Guid pilotId);
    IAsyncEnumerable<RaceInformation> GetRaceInformationForPilotAfterDate(Guid pilotId, DateTime afterDate);
}
