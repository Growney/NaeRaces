using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class PilotClubDetailsProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotClubDetailsProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private async Task When(PilotClubMembershipConfirmed e)
    {
        await UpsertPilotClubDetails(e.PilotId, e.ClubId, e.MembershipLevelId, e.ValidUntil);
    }

    private async Task When(PilotClubMembershipManuallyConfirmed e)
    {
        await UpsertPilotClubDetails(e.PilotId, e.ClubId, e.MembershipLevelId, e.ValidUntil);
    }

    private async Task When(PilotClubMembershipCancelled e)
    {
        await RemovePilotClubDetails(e.PilotId, e.ClubId);
    }

    private async Task When(PilotClubMembershipRevoked e)
    {
        await RemovePilotClubDetails(e.PilotId, e.ClubId);
    }

    private async Task When(ClubDetailsChanged e)
    {
        var records = await _dbContext.PilotClubDetails
            .Where(x => x.ClubId == e.ClubId)
            .ToListAsync();

        foreach (var record in records)
        {
            record.ClubName = e.Name;
            record.ClubCode = e.Code;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubHomeLocationSet e)
    {
        ClubLocation? location = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == e.ClubId && cl.LocationId == e.LocationId);

        var records = await _dbContext.PilotClubDetails
            .Where(x => x.ClubId == e.ClubId)
            .ToListAsync();

        foreach (var record in records)
        {
            record.HomeLocationName = location?.Name;
            record.HomeLocationAddressLine1 = location?.AddressLine1;
            record.HomeLocationAddressLine2 = location?.AddressLine2;
            record.HomeLocationCity = location?.City;
            record.HomeLocationPostcode = location?.Postcode;
            record.HomeLocationCounty = location?.County;
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

        var records = await _dbContext.PilotClubDetails
            .Where(x => x.ClubId == e.ClubId)
            .ToListAsync();

        foreach (var record in records)
        {
            record.HomeLocationName = e.NewLocationName;
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

        var records = await _dbContext.PilotClubDetails
            .Where(x => x.ClubId == e.ClubId)
            .ToListAsync();

        foreach (var record in records)
        {
            record.HomeLocationAddressLine1 = e.NewAddress.AddressLine1;
            record.HomeLocationAddressLine2 = e.NewAddress.AddressLine2;
            record.HomeLocationCity = e.NewAddress.City;
            record.HomeLocationPostcode = e.NewAddress.Postcode;
            record.HomeLocationCounty = e.NewAddress.County;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubMembershipLevelRenamed e)
    {
        var records = await _dbContext.PilotClubDetails
            .Where(x => x.ClubId == e.ClubId && x.MembershipLevelId == e.MembershipLevelId)
            .ToListAsync();

        foreach (var record in records)
        {
            record.MembershipLevelName = e.NewName;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task UpsertPilotClubDetails(Guid pilotId, Guid clubId, int membershipLevelId, DateTime validUntil)
    {
        ClubMembershipLevel? membershipLevel = await _dbContext.ClubMembershipLevels
            .SingleOrDefaultAsync(ml => ml.ClubId == clubId && ml.MembershipLevelId == membershipLevelId);

        PilotClubDetails? existing = await _dbContext.PilotClubDetails
            .SingleOrDefaultAsync(x => x.PilotId == pilotId && x.ClubId == clubId);

        if (existing != null)
        {
            existing.MembershipLevelId = membershipLevelId;
            existing.MembershipLevelName = membershipLevel?.Name;
            existing.MembershipValidUntil = validUntil;
            await _dbContext.SaveChangesAsync();
            return;
        }

        ClubDetails? clubDetails = await _dbContext.ClubDetails
            .SingleOrDefaultAsync(x => x.Id == clubId);

        ClubLocation? homeLocation = await _dbContext.ClubLocations
            .SingleOrDefaultAsync(cl => cl.ClubId == clubId && cl.IsHomeLocation);

        PilotClubDetails record = new()
        {
            PilotId = pilotId,
            ClubId = clubId,
            ClubName = clubDetails?.Name,
            ClubCode = clubDetails?.Code,
            HomeLocationName = homeLocation?.Name,
            HomeLocationAddressLine1 = homeLocation?.AddressLine1,
            HomeLocationAddressLine2 = homeLocation?.AddressLine2,
            HomeLocationCity = homeLocation?.City,
            HomeLocationPostcode = homeLocation?.Postcode,
            HomeLocationCounty = homeLocation?.County,
            MembershipLevelId = membershipLevelId,
            MembershipLevelName = membershipLevel?.Name,
            MembershipValidUntil = validUntil
        };

        _dbContext.PilotClubDetails.Add(record);
        await _dbContext.SaveChangesAsync();
    }

    private async Task RemovePilotClubDetails(Guid pilotId, Guid clubId)
    {
        PilotClubDetails? existing = await _dbContext.PilotClubDetails
            .SingleOrDefaultAsync(x => x.PilotId == pilotId && x.ClubId == clubId);

        if (existing != null)
        {
            _dbContext.PilotClubDetails.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }
    }
}
