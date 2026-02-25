using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;

namespace NaeRaces.Query.Abstractions;

public interface IRacePackageQueryHandler
{
    Task<RacePackage?> GetRacePackage(Guid raceId, int packageId);
    IAsyncEnumerable<RacePackage> GetRacePackages(Guid raceId);
}
