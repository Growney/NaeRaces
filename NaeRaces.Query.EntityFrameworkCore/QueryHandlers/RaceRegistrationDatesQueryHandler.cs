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
            .Include(x => x.EarlyRegistrationDates)
            .SingleOrDefaultAsync(x => x.RaceId == raceId);

        if (dbRaceRegistrationDates == null)
            return null;

        var earlyRegistrationDates = dbRaceRegistrationDates.EarlyRegistrationDates
            .Select(erd => new RaceEarlyRegistrationDate(
                erd.EarlyRegistrationId,
                erd.RegistrationOpenDate,
                erd.PilotPolicyId,
                erd.PolicyVersion
            ))
            .ToList();

        return new RaceRegistrationDates(
            dbRaceRegistrationDates.RaceId,
            dbRaceRegistrationDates.RegistrationOpenDate,
            dbRaceRegistrationDates.RaceValidationPolicyId,
            dbRaceRegistrationDates.RaceValidationPolicyVersion,
            earlyRegistrationDates
        );
    }

    public async Task<RaceEarlyRegistrationDate?> GetEarlyRegistrationDate(Guid raceId, int earlyRegistrationId)
    {
        var dbEarlyRegistrationDate = await _dbContext.Set<Models.RaceEarlyRegistrationDate>()
            .SingleOrDefaultAsync(x => x.RaceId == raceId && x.EarlyRegistrationId == earlyRegistrationId);

        if (dbEarlyRegistrationDate == null)
            return null;

        return new RaceEarlyRegistrationDate(
            dbEarlyRegistrationDate.EarlyRegistrationId,
            dbEarlyRegistrationDate.RegistrationOpenDate,
            dbEarlyRegistrationDate.PilotPolicyId,
            dbEarlyRegistrationDate.PolicyVersion
        );
    }

    public IAsyncEnumerable<RaceEarlyRegistrationDate> GetEarlyRegistrationDates(Guid raceId) =>
        _dbContext.Set<Models.RaceEarlyRegistrationDate>()
            .Where(x => x.RaceId == raceId)
            .Select(dbEarlyRegistrationDate => new RaceEarlyRegistrationDate(
                dbEarlyRegistrationDate.EarlyRegistrationId,
                dbEarlyRegistrationDate.RegistrationOpenDate,
                dbEarlyRegistrationDate.PilotPolicyId,
                dbEarlyRegistrationDate.PolicyVersion
            ))
            .ToAsyncEnumerable();
}
