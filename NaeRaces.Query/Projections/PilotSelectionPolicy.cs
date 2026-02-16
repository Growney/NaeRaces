using NaeRaces.Events;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.Projections;

public class PilotSelectionPolicy
{
    private class UnbuiltPilotSelectionPolicyOperandStatement : IPilotSelectionPolicyStatement
    {
        public int LeftHandStatementId { get; }
        public string Operand { get; }
        public int RightHandStatementId { get; }
        public bool IsWithinBrackets { get; }
        public UnbuiltPilotSelectionPolicyOperandStatement(int leftHandStatementId, string operand, int rightHandStatementId, bool isWithinBrackets)
        {
            LeftHandStatementId = leftHandStatementId;
            Operand = operand;
            RightHandStatementId = rightHandStatementId;
            IsWithinBrackets = isWithinBrackets;
        }
        public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime onDate)
        {
            throw new InvalidOperationException("Cannot evaluate validity of an unbuilt operand statement. This statement needs to be built into a PilotSelectionPolicyOperandStatement first.");
        }
    }

    private readonly Dictionary<int, IPilotSelectionPolicyStatement> _statements = new();
    private Guid _policyId;
    private Guid _clubId;
    private int? _rootStatementId;

    public PilotSelectionPolicyStatementTree? StatementTree => _rootStatementId.HasValue 
        ? new PilotSelectionPolicyStatementTree(_policyId, _clubId, BuildStatement(_rootStatementId.Value))
        : null;

    private void When(PilotSelectionPolicyCreated e)
    {
        _policyId = e.PilotSelectionPolicyId;
        _clubId = e.ClubId;
    }

    private void When(PilotSelectionPolicyMinimumAgeRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new PilotSelectionPolicyMinimumAgeStatement(_clubId,
            e.MinimumAge,
            e.ValidationPolicy
        );
    }

    private void When(PilotSelectionPolicyMaximumAgeRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new PilotSelectionPolicyMaximumAgeStatement(_clubId,
            e.MaximumAge,
            e.ValidationPolicy
        );
    }

    private void When(PilotSelectionPolicyInsuranceProviderRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new PilotSelectionPolicyInsuranceProviderRequirementStatement(_clubId,
            e.InsuranceProvider,
            e.ValidationPolicy
        );
    }

    private void When(PilotSelectionPolicyGovernmentDocumentValidationRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new PilotSelectionPolicyGovernmentDocumentRequirementStatement(_clubId,
            e.GovernmentDocument,
            e.ValidationPolicy
        );
    }

    private void When(PilotSelectionPolicyClubRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new PilotSelectionPolicyClubRequirementStatement(
            e.ClubId
        );
    }

    private void When(PilotSelectionPolicyClubMembershipLevelRequirementAdded e)
    {
        _statements[e.PolicyStatementId] = new PilotSelectionPolicyClubMembershipLevelRequirementStatement(
            e.ClubId,
            e.MembershipLevel
        );
    }

    private void When(PilotSelectionPolicyStatementAdded e)
    {
        // Store as a placeholder that will be resolved when building the tree
        _statements[e.PolicyStatementId] = new UnbuiltPilotSelectionPolicyOperandStatement(
            e.LeftHandStatementId,
            e.Operand,
            e.RightHandStatementId,
            e.IsWithinBrackets
        );
    }

    private void When(PilotSelectionPolicyStatementRemoved e)
    {
        _statements.Remove(e.PolicyStatementId);
    }

    private void When(PilotSelectionPolicyRootStatementSet e)
    {
        _rootStatementId = e.RootPolicyStatementId;
    }

    private IPilotSelectionPolicyStatement BuildStatement(int statementId)
    {
        if (!_statements.TryGetValue(statementId, out var statement))
        {
            throw new InvalidOperationException($"Statement {statementId} not found in policy {_policyId}");
        }

        // If it's an operand statement, recursively build the left and right statements
        if (statement is UnbuiltPilotSelectionPolicyOperandStatement operandStatement)
        {
            var leftStatement = BuildStatement(operandStatement.LeftHandStatementId);
            var rightStatement = BuildStatement(operandStatement.RightHandStatementId);

            return new PilotSelectionPolicyOperandStatement(
                leftStatement,
                operandStatement.Operand,
                rightStatement,
                operandStatement.IsWithinBrackets
            );
        }

        return statement;
    }
}
