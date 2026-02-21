using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IRaceDetailsQueryHandler
{
    Task<bool> DoesRaceExist(Guid raceId);
}
