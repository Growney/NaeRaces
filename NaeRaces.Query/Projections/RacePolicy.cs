using NaeRaces.Events;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.Projections;

public class RacePolicy
{
    private class UnbuiltRacePolicyOperandStatement : IRacePolicyStatement
    {
        public int LeftHandStatementId { get; }
        public string Operand { get; }
        public int RightHandStatementId { get; }
        public bool IsWithinBrackets { get; }
        public UnbuiltRacePolicyOperandStatement(int leftHandStatementId, string operand, int rightHandStatementId, bool isWithinBrackets)
        {
            LeftHandStatementId = leftHandStatementId;
            Operand = operand;
            RightHandStatementId = rightHandStatementId;
            IsWithinBrackets = isWithinBrackets;
        }
        public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime onDate)
        {
            throw new InvalidOperationException("Cannot evaluate validity of an unbuilt operand statement. This statement needs to be built into a RacePolicyOperandStatement first.");
        }
    }

    private readonly Dictionary<int, IRacePolicyStatement> _statements = new();
    private Guid _policyId;
    private Guid _clubId;
    private int? _rootStatementId;

    public RacePolicyStatementTree? StatementTree => _rootStatementId.HasValue 
        ? new RacePolicyStatementTree(_policyId, _clubId, BuildStatement(_rootStatementId.Value))
        : null;

    private void When(RacePolicyCreated e)
    {
        _policyId = e.RacePolicyId;
        _clubId = e.ClubId;
    }

    private void When(RacePolicyMinimumAgeRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new RacePolicyMinimumAgeStatement(_clubId,
            e.MinimumAge,
            e.ValidationPolicy
        );
    }

    private void When(RacePolicyMaximumAgeRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new RacePolicyMaximumAgeStatement(_clubId,
            e.MaximumAge,
            e.ValidationPolicy
        );
    }

    private void When(RacePolicyInsuranceProviderRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new RacePolicyInsuranceProviderRequirementStatement(_clubId,
            e.InsuranceProvider,
            e.ValidationPolicy
        );
    }

    private void When(RacePolicyGovernmentDocumentValidationRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new RacePolicyGovernmentDocumentRequirementStatement(_clubId,
            e.GovernmentDocument,
            e.ValidationPolicy
        );
    }

    private void When(RacePolicyClubRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new RacePolicyClubRequirementStatement(
            e.ClubId
        );
    }

    private void When(RacePolicyClubMembershipLevelRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new RacePolicyClubMembershipLevelRequirementStatement(
            e.ClubId,
            e.MembershipLevel
        );
    }

    private void When(RacePolicyStatementAdded e)
    {
        // Store as a placeholder that will be resolved when building the tree
        _statements[e.PolicyStatementId] = new UnbuiltRacePolicyOperandStatement(
            e.LeftHandStatementId,
            e.Operand,
            e.RightHandStatementId,
            e.IsWithinBrackets
        );
    }

    private void When(RacePolicyStatementRemoved e)
    {
        _statements.Remove(e.PolicyStatementId);
    }

    private void When(RacePolicyRootStatementSet e)
    {
        _rootStatementId = e.RootPolicyStatementId;
    }

    private IRacePolicyStatement BuildStatement(int statementId)
    {
        if (!_statements.TryGetValue(statementId, out var statement))
        {
            throw new InvalidOperationException($"Statement {statementId} not found in policy {_policyId}");
        }

        // If it's an operand statement, recursively build the left and right statements
        if (statement is UnbuiltRacePolicyOperandStatement operandStatement)
        {
            var leftStatement = BuildStatement(operandStatement.LeftHandStatementId);
            var rightStatement = BuildStatement(operandStatement.RightHandStatementId);

            return new RacePolicyOperandStatement(
                leftStatement,
                operandStatement.Operand,
                rightStatement,
                operandStatement.IsWithinBrackets
            );
        }

        return statement;
    }
}

