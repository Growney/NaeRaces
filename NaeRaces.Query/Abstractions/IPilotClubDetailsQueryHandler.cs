using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IPilotClubDetailsQueryHandler
{
    IAsyncEnumerable<PilotClubDetails> GetPilotClubs(Guid pilotId);
}
