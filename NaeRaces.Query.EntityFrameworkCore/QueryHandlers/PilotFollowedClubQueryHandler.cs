using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotFollowedClubQueryHandler : IPilotFollowedClubQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotFollowedClubQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<PilotFollowedClub> GetFollowedClubs(Guid pilotId)
    {
        return _dbContext.PilotFollowedClubs
            .Where(x => x.PilotId == pilotId)
            .Select(x => new PilotFollowedClub(x.PilotId, x.ClubId))
            .AsAsyncEnumerable();
    }

    public Task<bool> IsFollowingClub(Guid pilotId, Guid clubId)
    {
        return _dbContext.PilotFollowedClubs
            .AnyAsync(x => x.PilotId == pilotId && x.ClubId == clubId);
    }
}
