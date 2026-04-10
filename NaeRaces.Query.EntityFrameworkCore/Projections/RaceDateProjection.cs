using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RaceDateProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceDateProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(RaceDateScheduled e)
    {
        RaceDate raceDate = new()
        {
            RaceId = e.RaceId,
            RaceDateId = e.RaceDateId,
            Start = e.Start,
            End = e.End,
            Cancelled = false
        };

        _dbContext.RaceDates.Add(raceDate);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDateRescheduled e)
    {
        RaceDate? raceDate = await _dbContext.RaceDates
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RaceDateId == e.RaceDateId);

        if (raceDate != null)
        {
            raceDate.Start = e.Start;
            raceDate.End = e.End;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task When(RaceDateCancelled e)
    {
        RaceDate? raceDate = await _dbContext.RaceDates
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RaceDateId == e.RaceDateId);

        if (raceDate != null)
        {
            raceDate.Cancelled = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
