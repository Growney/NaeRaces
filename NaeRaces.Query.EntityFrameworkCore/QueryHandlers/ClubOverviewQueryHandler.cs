using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class ClubOverviewQueryHandler : IClubOverviewQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubOverviewQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ClubOverview?> GetClubOverview(Guid clubId)
    {
        var x = await _dbContext.ClubOverviews.FirstOrDefaultAsync(c => c.ClubId == clubId);
        return x == null ? null : ToQueryModel(x);
    }

    public IAsyncEnumerable<ClubOverview> GetAllClubs()
    {
        return _dbContext.ClubOverviews
            .Select(x => ToQueryModel(x))
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<ClubOverview> GetTopClubsByMemberCount(int count)
    {
        return _dbContext.ClubOverviews
            .OrderByDescending(x => x.TotalMemberCount)
            .Take(count)
            .Select(x => ToQueryModel(x))
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<ClubOverview> GetClubsWithRacesAfter(DateTime date)
    {
        return _dbContext.ClubOverviews
            .Where(x => _dbContext.RaceInformation.Any(r => r.ClubId == x.ClubId && r.LastRaceDateEnd >= date))
            .Select(x => ToQueryModel(x))
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<ClubOverview> SearchClubs(string searchTerm)
    {
        return _dbContext.ClubOverviews
            .Where(x => (x.Name != null && x.Name.Contains(searchTerm)) ||
                        (x.Code != null && x.Code.Contains(searchTerm)))
            .Select(x => ToQueryModel(x))
            .AsAsyncEnumerable();
    }

    private static ClubOverview ToQueryModel(Models.ClubOverview x)
    {
        HomeLocation? homeLocation = x.HomeLocationName != null
            ? new HomeLocation(x.HomeLocationName, x.HomeLocationAddressLine1, x.HomeLocationAddressLine2, x.HomeLocationCity, x.HomeLocationPostcode, x.HomeLocationCounty)
            : null;

        return new ClubOverview(
            x.ClubId,
            x.Name ?? string.Empty,
            x.Code ?? string.Empty,
            homeLocation,
            x.TotalMemberCount);
    }
}
