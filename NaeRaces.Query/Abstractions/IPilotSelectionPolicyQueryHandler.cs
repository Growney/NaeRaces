using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IPilotSelectionPolicyQueryHandler
{
    Task<PilotSelectionPolicyDetails?> GetPolicyDetails(Guid policyId, Guid clubId);
    IAsyncEnumerable<PilotSelectionPolicyDetails> GetClubPolicies(Guid clubId);
    IAsyncEnumerable<PilotPolicyStatementDetails> GetPolicyStatements(Guid policyId);
}
