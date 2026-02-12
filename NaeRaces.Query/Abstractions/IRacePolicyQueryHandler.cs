using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IRacePolicyQueryHandler
{
    Task<RacePolicyDetails?> GetPolicyDetails(Guid policyId, Guid clubId);
}
