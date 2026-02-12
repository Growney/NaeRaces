using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class ClubLocationQueryHandler : IClubLocationQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubLocationQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<bool> DoesLocationExist(Guid clubId, int locationId)
        => _dbContext.ClubLocations.AnyAsync(cl => cl.ClubId == clubId && cl.LocationId == locationId);

    public Task<bool> IsLocationInUse(Guid clubId, int locationId)
        => _dbContext.RaceDetails.AnyAsync(rd => rd.ClubId == clubId && rd.LocationId == locationId && !rd.IsCancelled);
}
