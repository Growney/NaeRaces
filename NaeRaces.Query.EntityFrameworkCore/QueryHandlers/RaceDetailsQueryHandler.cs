using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RaceDetailsQueryHandler : IRaceDetailsQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceDetailsQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<bool> DoesRaceExist(Guid raceId)
    {
        return await _dbContext.RaceDetails.AnyAsync(x => x.Id == raceId);
    }
}
