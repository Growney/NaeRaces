using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RaceDateQueryHandler : IRaceDateQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceDateQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<RaceDate> GetRaceDates(Guid raceId) =>
        _dbContext.RaceDates
            .Where(x => x.RaceId == raceId && !x.Cancelled)
            .OrderBy(x => x.Start)
            .Select(x => new RaceDate(x.RaceId, x.RaceDateId, x.Start, x.End, x.Cancelled))
            .AsAsyncEnumerable();
}
