using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RaceDetailsProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceDetailsProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(RacePlanned e)
    {
        RaceDetails details = new()
        {
            Id = e.RaceId,
            Name = e.Name,
            IsTeamRace = false,
            ClubId = e.ClubId,
            LocationId = e.LocationId,
            NumberOfRaceDates = 0,
            IsDetailsPublished = false,
            IsPublished = false,
            IsCancelled = false
        };

        _dbContext.RaceDetails.Add(details);

        return _dbContext.SaveChangesAsync();
    }

    private Task When(TeamRacePlanned e)
    {
        RaceDetails details = new()
        {
            Id = e.RaceId,
            Name = e.Name,
            IsTeamRace = true,
            NumberOfRaceDates = 0,
            IsDetailsPublished = false,
            IsPublished = false,
            IsCancelled = false
        };

        _dbContext.RaceDetails.Add(details);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDescriptionSet e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            // RaceDetails model doesn't have Description field, but handling event for completeness
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDateScheduled e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.NumberOfRaceDates++;
            
            if (details.NumberOfRaceDates == 1 || e.Start < details.FirstRaceDateStart)
            {
                details.FirstRaceDateStart = e.Start;
            }
            
            if (details.NumberOfRaceDates == 1 || e.End > details.LastRaceDateEnd)
            {
                details.LastRaceDateEnd = e.End;
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDateRescheduled e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            if (e.Start < details.FirstRaceDateStart)
            {
                details.FirstRaceDateStart = e.Start;
            }
            
            if (e.End > details.LastRaceDateEnd)
            {
                details.LastRaceDateEnd = e.End;
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDateCancelled e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.NumberOfRaceDates--;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDetailsPublished e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.IsDetailsPublished = true;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RacePublished e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.IsPublished = true;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceCancelled e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.IsCancelled = true;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceValidationPolicySet e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.PilotPolicyId = e.PolicyId;
            details.PilotPolicyVersion = e.PolicyVersion;
        }
        await _dbContext.SaveChangesAsync();
    }
    private async Task When(RaceValidationPolicyRemoved e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.PilotPolicyId = null;
            details.PilotPolicyVersion = null;
        }
        await _dbContext.SaveChangesAsync();
    }
    private async Task When(RaceValidationPolicyMigratedToVersion e)
    {
        RaceDetails? details = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == e.RaceId);
        if (details != null)
        {
            details.PilotPolicyId = e.PolicyId;
            details.PilotPolicyVersion = e.PolicyVersion;
        }
        await _dbContext.SaveChangesAsync();
    }
}
