using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotSelectionPolicyMaximumAgeStatement(Guid ClubId, int MaximumAge, string ValidationPolicy) : IPilotSelectionPolicyStatement
{
    public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime onDate)
    {
        if (!pilotValidationDetails.DateOfBirth.HasValue)
        {
            return "PILOT_NO_DOB"; // Cannot validate age without date of birth
        }
        var age = CalculateAge(pilotValidationDetails.DateOfBirth.Value, DateTime.UtcNow);
        if(age > MaximumAge)
        {
            return "PILOT_TOO_OLD"; // Pilot exceeds maximum age
        }

        if(!pilotValidationDetails.AgeValidations.IsValidForPolicy(ValidationPolicy, ClubId, pilotValidationDetails.PilotClubs.Select(x => x.ClubId)))
        {
            return "AGE_FAILED_MAXIMUM_VALIDATION_POLICY_" + ValidationPolicy;
        }

        return null;
    }

    private int CalculateAge(DateTime birthDate, DateTime currentDate)
    {
        int age = currentDate.Year - birthDate.Year;
        if (currentDate < birthDate.AddYears(age))
        {
            age--;
        }
        return age;
    }
}
