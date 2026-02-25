using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IRaceDiscountQueryHandler
{
    IAsyncEnumerable<RaceDiscount> GetRaceDiscounts(Guid raceId, string currency);
}
