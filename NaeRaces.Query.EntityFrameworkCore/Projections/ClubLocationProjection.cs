using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class ClubLocationProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubLocationProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(ClubLocationAdded e)
    {
        ClubLocation location = new()
        {
            ClubId = e.ClubId,
            LocationId = e.LocationId,
            Name = e.LocationName,
            LocationInformation = e.LocationInformation,
            AddressLine1 = e.Address.AddressLine1,
            AddressLine2 = e.Address.AddressLine2,
            City = e.Address.City,
            Postcode = e.Address.Postcode,
            County = e.Address.County,
            IsHomeLocation = false
        };

        _dbContext.ClubLocations.Add(location);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationRemoved e)
    {
        ClubLocation? location = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        if (location != null)
        {
            _dbContext.ClubLocations.Remove(location);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationRenamed e)
    {
        ClubLocation? location = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        if (location != null)
        {
            location.Name = e.NewLocationName;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationAddressChanged e)
    {
        ClubLocation? location = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        if (location != null)
        {
            location.AddressLine1 = e.NewAddress.AddressLine1;
            location.AddressLine2 = e.NewAddress.AddressLine2;
            location.City = e.NewAddress.City;
            location.Postcode = e.NewAddress.Postcode;
            location.County = e.NewAddress.County;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationInformationChanged e)
    {
        ClubLocation? location = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        if (location != null)
        {
            location.LocationInformation = e.NewLocationInformation;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationTimezoneOffsetSet e)
    {
        ClubLocation? location = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        if (location != null)
        {
            location.TimezoneOffsetMinutes = e.TimezoneOffsetMinutes;
            location.UseDaylightSavings = e.UseDaylightSavings;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubHomeLocationSet e)
    {
        // Clear any existing home location for this club
        var existingHomeLocations = await _dbContext.ClubLocations
            .Where(cl => cl.ClubId == e.ClubId && cl.IsHomeLocation)
            .ToListAsync();

        foreach (var existingHome in existingHomeLocations)
        {
            existingHome.IsHomeLocation = false;
        }

        // Set the new home location
        ClubLocation? newHomeLocation = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        if (newHomeLocation != null)
        {
            newHomeLocation.IsHomeLocation = true;
        }

        await _dbContext.SaveChangesAsync();
    }
}
