using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RaceRegistrationDatesQueryHandler : IRaceRegistrationDatesQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceRegistrationDatesQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<RaceRegistrationDates?> GetRaceRegistrationDates(Guid raceId)
    {
        var dbRaceRegistrationDates = await _dbContext.RaceRegistrationDates
            .SingleOrDefaultAsync(x => x.RaceId == raceId);

        if (dbRaceRegistrationDates == null)
            return null;

        return new RaceRegistrationDates(
            dbRaceRegistrationDates.RaceId,
            dbRaceRegistrationDates.RaceValidationPolicyId,
            dbRaceRegistrationDates.RaceValidationPolicyVersion
        );
    }
}
