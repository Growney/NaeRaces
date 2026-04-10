using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;

namespace NaeRaces.Query.Abstractions;

public interface IRaceDateQueryHandler
{
    IAsyncEnumerable<RaceDate> GetRaceDates(Guid raceId);
}
