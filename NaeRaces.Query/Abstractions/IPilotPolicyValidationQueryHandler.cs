using System;
using System.Threading.Tasks;

namespace NaeRaces.Query.Abstractions;

public interface IPilotPolicyValidationQueryHandler
{
    Task<string?> ValidatePilotAgainstPolicy(Guid pilotId, Guid policyId, long policyVersion, DateTime onDate);
}
