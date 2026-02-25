using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IRaceDetailsQueryHandler
{
    Task<bool> DoesRaceExist(Guid raceId);
    Task<RaceRegistrationDetails?> GetRaceRegistrationDetails(Guid raceId);
}
