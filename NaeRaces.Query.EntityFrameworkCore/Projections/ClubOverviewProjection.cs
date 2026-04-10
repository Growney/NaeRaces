using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class ClubOverviewProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubOverviewProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(ClubFormed e)
    {
        ClubOverview overview = new()
        {
            ClubId = e.ClubId,
            Name = e.Name,
            Code = e.Code,
            TotalMemberCount = 0
        };

        _dbContext.ClubOverviews.Add(overview);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubDetailsChanged e)
    {
        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null)
        {
            overview.Name = e.Name;
            overview.Code = e.Code;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubHomeLocationSet e)
    {
        ClubLocation? location = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null)
        {
            overview.HomeLocationName = location?.Name;
            overview.HomeLocationAddressLine1 = location?.AddressLine1;
            overview.HomeLocationAddressLine2 = location?.AddressLine2;
            overview.HomeLocationCity = location?.City;
            overview.HomeLocationPostcode = location?.Postcode;
            overview.HomeLocationCounty = location?.County;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationRenamed e)
    {
        bool isHomeLocation = await _dbContext.ClubLocations
            .AnyAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId && cl.IsHomeLocation);

        if (!isHomeLocation)
        {
            return;
        }

        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null)
        {
            overview.HomeLocationName = e.NewLocationName;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationAddressChanged e)
    {
        bool isHomeLocation = await _dbContext.ClubLocations
            .AnyAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId && cl.IsHomeLocation);

        if (!isHomeLocation)
        {
            return;
        }

        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null)
        {
            overview.HomeLocationAddressLine1 = e.NewAddress.AddressLine1;
            overview.HomeLocationAddressLine2 = e.NewAddress.AddressLine2;
            overview.HomeLocationCity = e.NewAddress.City;
            overview.HomeLocationPostcode = e.NewAddress.Postcode;
            overview.HomeLocationCounty = e.NewAddress.County;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipConfirmed e)
    {
        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null)
        {
            overview.TotalMemberCount++;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipManuallyConfirmed e)
    {
        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null)
        {
            overview.TotalMemberCount++;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipCancelled e)
    {
        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null && overview.TotalMemberCount > 0)
        {
            overview.TotalMemberCount--;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipRevoked e)
    {
        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null && overview.TotalMemberCount > 0)
        {
            overview.TotalMemberCount--;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipExpired e)
    {
        ClubOverview? overview = await _dbContext.ClubOverviews.SingleOrDefaultAsync(x => x.ClubId == e.ClubId);
        if (overview != null && overview.TotalMemberCount > 0)
        {
            overview.TotalMemberCount--;
        }
        await _dbContext.SaveChangesAsync();
    }
}
