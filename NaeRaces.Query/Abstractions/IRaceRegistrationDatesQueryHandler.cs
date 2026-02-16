using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IRaceRegistrationDatesQueryHandler
{
    Task<RaceRegistrationDates?> GetRaceRegistrationDates(Guid raceId);
    Task<RaceEarlyRegistrationDate?> GetEarlyRegistrationDate(Guid raceId, int earlyRegistrationId);
    IAsyncEnumerable<RaceEarlyRegistrationDate> GetEarlyRegistrationDates(Guid raceId);
}
