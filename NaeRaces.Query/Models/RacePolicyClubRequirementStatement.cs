using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record RacePolicyClubRequirementStatement(Guid ClubId) : IRacePolicyStatement
{
    public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime OnDate)
    {
        if (!pilotValidationDetails.PilotClubs.Any(x => x.ClubId == ClubId))
        {
            return "PILOT_NOT_MEMBER_OF_REQUIRED_CLUB"; // Pilot is not a member of the required club
        }
        return null;
    }
}