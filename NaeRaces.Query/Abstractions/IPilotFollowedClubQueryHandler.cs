using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IPilotFollowedClubQueryHandler
{
    IAsyncEnumerable<PilotFollowedClub> GetFollowedClubs(Guid pilotId);
    Task<bool> IsFollowingClub(Guid pilotId, Guid clubId);
}
