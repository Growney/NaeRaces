using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotSelectionPolicyQueryHandler : IPilotSelectionPolicyQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotSelectionPolicyQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<PilotSelectionPolicyDetails?> GetPolicyDetails(Guid policyId, Guid clubId)
        => _dbContext.PilotSelectionPolicyDetails
            .Where(rp => rp.Id == policyId && rp.ClubId == clubId)
            .Select(rp => new PilotSelectionPolicyDetails(rp.Id, rp.ClubId, rp.Name, rp.Description, rp.LatestVersion, rp.RootStatementId))
            .SingleOrDefaultAsync();

    public IAsyncEnumerable<PilotSelectionPolicyDetails> GetClubPolicies(Guid clubId)
        => _dbContext.PilotSelectionPolicyDetails
            .Where(rp => rp.ClubId == clubId)
            .Select(rp => new PilotSelectionPolicyDetails(rp.Id, rp.ClubId, rp.Name, rp.Description, rp.LatestVersion, rp.RootStatementId))
            .AsAsyncEnumerable();

    public IAsyncEnumerable<PilotPolicyStatementDetails> GetPolicyStatements(Guid policyId)
        => _dbContext.PilotPolicyStatements
            .Where(s => s.PolicyId == policyId)
            .Select(s => new PilotPolicyStatementDetails(
                s.PolicyId,
                s.StatementId,
                s.StatementType,
                s.LeftHandStatementId,
                s.Operand,
                s.RightHandStatementId,
                s.IsWithinBrackets,
                s.MinimumAge,
                s.MaximumAge,
                s.InsuranceProvider,
                s.GovernmentDocument,
                s.RequiredClubId,
                s.RequiredMembershipLevelId,
                s.ValidationPolicy))
            .AsAsyncEnumerable();
}
