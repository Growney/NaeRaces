using NaeRaces.Query.Models;
using System;
using System.Threading.Tasks;

namespace NaeRaces.Query.Abstractions;

public interface IPilotRegistrationDetailsQueryHandler
{
    Task<PilotRegistrationDetails?> GetPilotRegistrationDetails(Guid pilotId, Guid raceId, string currency, DateTime onDate);
}
