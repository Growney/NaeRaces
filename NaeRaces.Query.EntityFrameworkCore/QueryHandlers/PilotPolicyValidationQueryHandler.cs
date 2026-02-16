using EventDbLite.Abstractions;
using EventDbLite.Aggregates;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Projections;
using System;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotPolicyValidationQueryHandler : IPilotPolicyValidationQueryHandler
{
    private readonly IPilotValidationQueryHandler _pilotValidationQueryHandler;
    private readonly IProjectionProvider _projectionProvider;

    public PilotPolicyValidationQueryHandler(
        IPilotValidationQueryHandler pilotValidationQueryHandler,
        IProjectionProvider projectionProvider)
    {
        _pilotValidationQueryHandler = pilotValidationQueryHandler ?? throw new ArgumentNullException(nameof(pilotValidationQueryHandler));
        _projectionProvider = projectionProvider ?? throw new ArgumentNullException(nameof(projectionProvider));
    }

    public async Task<string?> ValidatePilotAgainstPolicy(Guid pilotId, Guid policyId, long policyVersion, DateTime onDate)
    {
        var pilotValidationDetails = await _pilotValidationQueryHandler.GetPilotValidationDetails(pilotId);

        var streamName = GetPilotSelectionPolicyStreamName(policyId);
        var policy = await _projectionProvider.Load<PilotSelectionPolicy>(streamName, StreamPosition.WithVersion(policyVersion));

        if (policy.StatementTree == null)
        {
            return "Policy has no validation rules defined";
        }

        return policy.StatementTree.Statement.IsValidForPilot(pilotValidationDetails, onDate);
    }

    private static string GetPilotSelectionPolicyStreamName(Guid policyId) => $"PilotSelectionPolicy-{policyId}";
}
