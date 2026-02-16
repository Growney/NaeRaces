using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IRaceCostQueryHandler
{
    Task<RaceCost?> GetRaceCost(Guid raceId, string currency);
    IAsyncEnumerable<RaceCost> GetRaceCosts(Guid raceId);
    Task<RaceCostDiscount?> GetRaceDiscount(Guid raceId, string currency, Guid pilotPolicyId);
    IAsyncEnumerable<RaceCostDiscount> GetRaceDiscounts(Guid raceId, string currency);
}
