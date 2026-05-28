using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IPilotRelevantClubQueryHandler
{
    IAsyncEnumerable<PilotRelevantClub> GetPilotRelevantClubs(Guid pilotId);
}
