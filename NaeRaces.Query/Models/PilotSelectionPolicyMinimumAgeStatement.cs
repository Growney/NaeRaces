using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotSelectionPolicyMinimumAgeStatement(Guid ClubId, int MinimumAge, string ValidationPolicy) : IPilotSelectionPolicyStatement
{
    public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime onDate)
    {
        if (!pilotValidationDetails.DateOfBirth.HasValue)
        {
            return "PILOT_NO_DOB"; // Cannot validate age without date of birth
        }

        var age = CalculateAge(pilotValidationDetails.DateOfBirth.Value, onDate);
        if(age < MinimumAge)
        {
            return "PILOT_TOO_YOUNG"; // Pilot does not meet the minimum age requirement
        }

        if(!pilotValidationDetails.AgeValidations.IsValidForPolicy(ValidationPolicy, ClubId, pilotValidationDetails.PilotClubs.Select(x => x.ClubId)))
        {
            return "AGE_FAILED_MINIMUM_VALIDATION_POLICY_" + ValidationPolicy; // Pilot does not meet the age validation policy
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
