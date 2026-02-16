using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotSelectionPolicyGovernmentDocumentRequirementStatement(Guid ClubId, string GovernmentDocument, string ValidationPolicy) : IPilotSelectionPolicyStatement
{
    public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime OnDate)
    {
        if (!pilotValidationDetails.GovernmentDocuments.Contains(GovernmentDocument))
        {
            return $"PILOT_MISSING_GOVERNMENT_DOCUMENT_" + GovernmentDocument;
        }
        if (!pilotValidationDetails.GovernmentDocumentValidations.Where(x => x.Document == GovernmentDocument && x.ValidUntil >= OnDate)
            .IsValidForPolicy(ValidationPolicy, ClubId, pilotValidationDetails.PilotClubs.Select(x=>x.ClubId)))
        {
            return "PILOT_GOVERNMENT_DOCUMENT_" + GovernmentDocument + "_FAILED_VALIDATION_POLICY" + ValidationPolicy;
        }
        return null;
    }
}
