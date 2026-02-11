using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class PilotDetailsProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotDetailsProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(PilotRegistered e)
    {
        PilotDetails details = new()
        {
            Id = e.PilotId,
            Callsign = e.CallSign
        };

        _dbContext.PilotDetails.Add(details);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotCallSignChanged e)
    {
        PilotDetails? details = await _dbContext.PilotDetails.SingleOrDefaultAsync(x => x.Id == e.PilotId);
        if(details != null)
        {
            details.Callsign = e.NewCallSign;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotNameSet e)
    {
        PilotDetails? details = await _dbContext.PilotDetails.SingleOrDefaultAsync(x => x.Id == e.PilotId);
        if (details != null)
        {
            details.Name = e.Name;
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotNationalitySet e)
    {
        PilotDetails? details = await _dbContext.PilotDetails.SingleOrDefaultAsync(x => x.Id == e.Pilot);
        if (details != null)
        {
            details.Nationality = e.Nationality;
        }
        await _dbContext.SaveChangesAsync();
    }
}
