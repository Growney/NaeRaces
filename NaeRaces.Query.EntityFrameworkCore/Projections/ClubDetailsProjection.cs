using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class ClubDetailsProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubDetailsProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(ClubFormed e)
    {
        ClubDetails details = new()
        {
            Id = e.ClubId,
            Code = e.Code,
            Name = e.Name,
            FounderPilotId = e.FounderPilotId
        };

        _dbContext.ClubDetails.Add(details);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubDetailsChanged e)
    {
        ClubDetails? details = await _dbContext.ClubDetails.SingleOrDefaultAsync(x => x.Id == e.ClubId);
        if (details != null)
        {
            details.Code = e.Code;
            details.Name = e.Name;
        }
        await _dbContext.SaveChangesAsync();
    }
}
