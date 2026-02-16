using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IPilotSelectionPolicyQueryHandler
{
    Task<PilotSelectionPolicyDetails?> GetPolicyDetails(Guid policyId, Guid clubId);
}
