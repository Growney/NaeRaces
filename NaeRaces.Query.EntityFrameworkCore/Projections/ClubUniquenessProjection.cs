using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class ClubUniquenessProjection
{

    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubUniquenessProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private async Task When(Events.ClubFormed formed)
    {
        ClubUniquenessDetails details = new()
        {
            Id = formed.ClubId,
            Code = formed.Code,
            Name = formed.Name
        };

        _dbContext.ClubUniquenessDetails.Add(details);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(Events.ClubDetailsChanged detailsChanged)
    {
        ClubUniquenessDetails? existingDetails = await _dbContext.ClubUniquenessDetails.SingleOrDefaultAsync(x => x.Id == detailsChanged.ClubId);
        if (existingDetails != null)
        {
            existingDetails.Code = detailsChanged.Code;
            existingDetails.Name = detailsChanged.Name;
        }

        await _dbContext.SaveChangesAsync();
    }
}
