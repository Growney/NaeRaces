using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.ValueTypes;

public struct TeamMemberRegistration
{
    public Guid RegistrationId { get; }
    public Guid PilotId { get; }
    public int RacePackageId { get; }

    private TeamMemberRegistration(Guid registrationId, Guid pilotId, int racePackageId)
    {
        RegistrationId = registrationId;
        PilotId = pilotId;
        RacePackageId = racePackageId;
    }

    public static TeamMemberRegistration Create(Guid registrationId, Guid pilotId, int racePackageId)
    {
        if (registrationId == Guid.Empty)
        {
            throw new ArgumentException("Registration ID cannot be empty.", nameof(registrationId));
        }
        if (pilotId == Guid.Empty)
        {
            throw new ArgumentException("Pilot ID cannot be empty.", nameof(pilotId));
        }
        if (racePackageId <= 0)
        {
            throw new ArgumentException("Race package ID must be greater than zero.", nameof(racePackageId));
        }

        return new TeamMemberRegistration(registrationId, pilotId, racePackageId);
    }
}
