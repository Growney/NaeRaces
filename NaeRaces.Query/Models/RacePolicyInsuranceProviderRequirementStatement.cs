using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record RacePolicyInsuranceProviderRequirementStatement(Guid ClubId, string InsuranceProvider, string ValidationPolicy) : IRacePolicyStatement
{
    public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime onDate)
    {
        if (!pilotValidationDetails.InsuranceProviders.Contains(InsuranceProvider))
        {
            return $"PILOT_MISSING_INSURANCE_FROM_PROVIDER_" + InsuranceProvider;
        }

        if (!pilotValidationDetails.InsuranceProviderValidations.Where(x => x.Provider == InsuranceProvider && x.ValidUntil >= onDate)
            .IsValidForPolicy(ValidationPolicy, ClubId, pilotValidationDetails.PilotClubs.Select(x => x.ClubId)))
        {
            return "PILOT_INSURANCE_PROVIDER_" + InsuranceProvider + "_FAILED_VALIDATION_POLICY" + ValidationPolicy; 
        }

        return null;
    }
}
