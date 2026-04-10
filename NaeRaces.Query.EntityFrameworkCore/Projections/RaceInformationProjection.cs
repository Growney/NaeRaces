using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RaceInformationProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceInformationProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private async Task When(RacePlanned e)
    {
        ClubDetails? club = await _dbContext.ClubDetails.SingleOrDefaultAsync(x => x.Id == e.ClubId);
        ClubLocation? location = await _dbContext.ClubLocations.SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.LocationId == e.LocationId);

        RaceInformation info = new()
        {
            Id = e.RaceId,
            Name = e.Name,
            ClubId = e.ClubId,
            ClubName = club?.Name,
            LocationId = e.LocationId,
            LocationName = location?.Name,
            NumberOfRaceDates = 0,
            RegisteredPilotCount = 0
        };

        _dbContext.RaceInformation.Add(info);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(TeamRacePlanned e)
    {
        ClubDetails? club = await _dbContext.ClubDetails.SingleOrDefaultAsync(x => x.Id == e.ClubId);
        ClubLocation? location = await _dbContext.ClubLocations.SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.LocationId == e.LocationId);

        RaceInformation info = new()
        {
            Id = e.RaceId,
            Name = e.Name,
            ClubId = e.ClubId,
            ClubName = club?.Name,
            LocationId = e.LocationId,
            LocationName = location?.Name,
            NumberOfRaceDates = 0,
            RegisteredPilotCount = 0
        };

        _dbContext.RaceInformation.Add(info);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubFormed e)
    {
        List<RaceInformation> races = await _dbContext.RaceInformation.Where(x => x.ClubId == e.ClubId).ToListAsync();
        foreach (RaceInformation race in races)
        {
            race.ClubName = e.Name;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubDetailsChanged e)
    {
        List<RaceInformation> races = await _dbContext.RaceInformation.Where(x => x.ClubId == e.ClubId).ToListAsync();
        foreach (RaceInformation race in races)
        {
            race.ClubName = e.Name;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationAdded e)
    {
        List<RaceInformation> races = await _dbContext.RaceInformation.Where(x => x.ClubId == e.ClubId && x.LocationId == e.LocationId).ToListAsync();
        foreach (RaceInformation race in races)
        {
            race.LocationName = e.LocationName;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubLocationRenamed e)
    {
        List<RaceInformation> races = await _dbContext.RaceInformation.Where(x => x.ClubId == e.ClubId && x.LocationId == e.LocationId).ToListAsync();
        foreach (RaceInformation race in races)
        {
            race.LocationName = e.NewLocationName;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDateScheduled e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.NumberOfRaceDates++;

            if (info.NumberOfRaceDates == 1 || e.Start < info.FirstRaceDateStart)
            {
                info.FirstRaceDateStart = e.Start;
            }

            if (info.NumberOfRaceDates == 1 || e.End > info.LastRaceDateEnd)
            {
                info.LastRaceDateEnd = e.End;
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDateRescheduled e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            if (e.Start < info.FirstRaceDateStart)
            {
                info.FirstRaceDateStart = e.Start;
            }

            if (e.End > info.LastRaceDateEnd)
            {
                info.LastRaceDateEnd = e.End;
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDescriptionSet e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.Description = e.Description;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RacePaymentDeadlineScheduled e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.PaymentDeadline = e.PaymentDeadline;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceGoNoGoScheduled e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.GoNoGoDate = e.GoNoGoDate;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceGoNoGoRescheduled e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.GoNoGoDate = e.GoNoGoDate;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceMinimumAttendeesSet e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.MinimumPilots = e.MinimumAttendees;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceMaximumAttendeesSet e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.MaximumPilots = e.MaximumAttendees;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(IndividualPilotRegisteredForRace e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.RegisteredPilotCount++;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(TeamRosterPilotRegisteredForRace e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.RegisteredPilotCount++;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceRegistrationCancelled e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.RegisteredPilotCount--;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RacePublished e)
    {
        RaceInformation? info = await _dbContext.RaceInformation.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (info != null)
        {
            info.IsPublished = true;
        }
        await _dbContext.SaveChangesAsync();
    }
}
