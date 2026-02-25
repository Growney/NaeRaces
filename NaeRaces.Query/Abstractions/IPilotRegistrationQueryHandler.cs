using NaeRaces.Query.Models;
using System;
using System.Threading.Tasks;

namespace NaeRaces.Query.Abstractions;

public interface IPilotRegistrationQueryHandler
{
    public enum RegistrationStatus
    {
        Success= 0,
        PilotDoesntMeetPackageRequirements = 1,
        PilotDoesntMeetRaceRequirements = 2,
        PackageRegistrationNotOpen = 3,
    }
    Task<RegistrationStatus> GetPilotPotentialRegistrationStatusForRace(Guid pilotId, Guid raceId, int racePackageId);
    Task<PilotRegistrationDetails?> GetPilotRegistrationDetails(Guid pilotId, Guid raceId, int racePackageId);
}
