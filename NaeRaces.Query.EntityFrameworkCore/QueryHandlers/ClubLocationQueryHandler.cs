using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

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

    public IAsyncEnumerable<ClubLocationDetail> GetClubLocations(Guid clubId)
        => _dbContext.ClubLocations
            .Where(cl => cl.ClubId == clubId)
            .Select(cl => ToLocationDetail(cl))
            .AsAsyncEnumerable();

    private static ClubLocationDetail ToLocationDetail(Models.ClubLocation cl)
        => new(
            cl.LocationId,
            cl.Name ?? string.Empty,
            cl.LocationInformation ?? string.Empty,
            cl.AddressLine1 ?? string.Empty,
            cl.AddressLine2,
            cl.City ?? string.Empty,
            cl.Postcode ?? string.Empty,
            cl.County ?? string.Empty,
            cl.IsHomeLocation);
}
