using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RacePackageQueryHandler : IRacePackageQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RacePackageQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<RacePackage?> GetRacePackage(Guid raceId, int packageId)
    {
        var dbPackage = await _dbContext.RacePackages
            .SingleOrDefaultAsync(x => x.RaceId == raceId && x.RacePackageId == packageId);

        if (dbPackage == null)
            return null;

        return new RacePackage(
            dbPackage.RaceId,
            dbPackage.RacePackageId,
            dbPackage.Name,
            dbPackage.Currency,
            dbPackage.Cost,
            dbPackage.ApplyDiscounts,
            dbPackage.RegistrationOpenDate,
            dbPackage.RegistrationCloseDate,
            dbPackage.IsRegistrationManuallyOpened,
            dbPackage.PilotPolicyId,
            dbPackage.PolicyVersion
        );
    }

    public IAsyncEnumerable<RacePackage> GetRacePackages(Guid raceId) =>
        _dbContext.RacePackages
            .Where(x => x.RaceId == raceId)
            .Select(dbPackage => new RacePackage(
                dbPackage.RaceId,
                dbPackage.RacePackageId,
                dbPackage.Name,
                dbPackage.Currency,     
                dbPackage.Cost,
                dbPackage.ApplyDiscounts,
                dbPackage.RegistrationOpenDate,
                dbPackage.RegistrationCloseDate,
                dbPackage.IsRegistrationManuallyOpened,
                dbPackage.PilotPolicyId,
                dbPackage.PolicyVersion
            ))
            .ToAsyncEnumerable();
}
