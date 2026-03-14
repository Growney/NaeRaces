using Microsoft.AspNetCore.Mvc;
using NaeRaces.Query.Abstractions;
using NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

namespace NaeRaces.WebAPI.Controllers;

public class PilotSelectionPolicyQueryController : Controller
{
    private readonly IPilotSelectionPolicyQueryHandler _pilotSelectionPolicyQueryHandler;

    public PilotSelectionPolicyQueryController(IPilotSelectionPolicyQueryHandler pilotSelectionPolicyQueryHandler)
    {
        _pilotSelectionPolicyQueryHandler = pilotSelectionPolicyQueryHandler ?? throw new ArgumentNullException(nameof(pilotSelectionPolicyQueryHandler));
    }

    [HttpGet("api/club/{clubId:guid}/query/pilotpolicies")]
    public async Task<IActionResult> GetClubPilotPoliciesAsync([FromRoute] Guid clubId)
    {
        var results = new List<PilotSelectionPolicyResponse>();
        await foreach (var policy in _pilotSelectionPolicyQueryHandler.GetClubPolicies(clubId))
        {
            results.Add(new PilotSelectionPolicyResponse
            {
                PolicyId = policy.Id,
                Name = policy.Name,
                Description = policy.Description,
                Version = policy.LatestVersion,
                RootStatementId = policy.RootStatementId
            });
        }
        return Ok(results);
    }

    [HttpGet("api/pilotselectionpolicy/{policyId:guid}/query/statements")]
    public async Task<IActionResult> GetPolicyStatementsAsync([FromRoute] Guid policyId)
    {
        var results = new List<PilotPolicyStatementResponse>();
        await foreach (var stmt in _pilotSelectionPolicyQueryHandler.GetPolicyStatements(policyId))
        {
            results.Add(new PilotPolicyStatementResponse
            {
                StatementId = stmt.StatementId,
                StatementType = stmt.StatementType,
                LeftHandStatementId = stmt.LeftHandStatementId,
                Operand = stmt.Operand,
                RightHandStatementId = stmt.RightHandStatementId,
                IsWithinBrackets = stmt.IsWithinBrackets,
                MinimumAge = stmt.MinimumAge,
                MaximumAge = stmt.MaximumAge,
                InsuranceProvider = stmt.InsuranceProvider,
                GovernmentDocument = stmt.GovernmentDocument,
                RequiredClubId = stmt.RequiredClubId,
                RequiredMembershipLevelId = stmt.RequiredMembershipLevelId,
                ValidationPolicy = stmt.ValidationPolicy
            });
        }
        return Ok(results);
    }
}
