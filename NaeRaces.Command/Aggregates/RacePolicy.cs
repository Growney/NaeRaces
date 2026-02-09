using EventDbLite.Aggregates;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Command.Aggregates;

public class RacePolicy : AggregateRoot<Guid>
{
    private ValueTypes.Name? _name;
    private string? _description;
    private Guid? _clubId;
    private int? _rootPolicyStatementId;
    private int _nextPolicyStatementId = 1;

    private enum StatementType
    {
        MinimumAge,
        MaximumAge,
        InsuranceProvider,
        GovernmentDocument,
        ClubMembership,
        ClubMembershipLevel,
        Composite
    }

    private readonly Dictionary<int, StatementType> _statementTypes = [];
    private readonly Dictionary<int, Statement> _compositeStatements = [];
    private readonly Dictionary<int, MinimumAgeRequirement> _minimumAgeRequirements = [];
    private readonly Dictionary<int, MaximumAgeRequirement> _maximumAgeRequirements = [];
    private readonly Dictionary<int, InsuranceProviderRequirement> _insuranceProviderRequirements = [];
    private readonly Dictionary<int, GovernmentDocumentRequirement> _governmentDocumentRequirements = [];
    private readonly Dictionary<int, ClubRequirement> _clubRequirements = [];
    private readonly Dictionary<int, ClubMembershipLevelRequirement> _clubMembershipLevelRequirements = [];

    private class MinimumAgeRequirement
    {
        public int MinimumAge { get; set; }
        public string ValidationPolicy { get; set; } = string.Empty;
    }

    private class MaximumAgeRequirement
    {
        public int MaximumAge { get; set; }
        public string ValidationPolicy { get; set; } = string.Empty;
    }

    private class InsuranceProviderRequirement
    {
        public string InsuranceProvider { get; set; } = string.Empty;
        public string ValidationPolicy { get; set; } = string.Empty;
    }

    private class GovernmentDocumentRequirement
    {
        public string GovernmentDocument { get; set; } = string.Empty;
        public string ValidationPolicy { get; set; } = string.Empty;
    }

    private class ClubRequirement
    {
        public Guid ClubId { get; set; }
    }

    private class ClubMembershipLevelRequirement
    {
        public Guid ClubId { get; set; }
        public int MembershipLevel { get; set; }
    }

    private class Statement
    {
        public int LeftHandStatementId { get; set; }
        public string Operand { get; set; } = string.Empty;
        public int RightHandStatementId { get; set; }
        public bool IsWithinBrackets { get; set; }
    }

    public RacePolicy(Guid racePolicyId, Guid clubId, ValueTypes.Name name, string description)
    {
        Raise(new RacePolicyCreated(racePolicyId, clubId, name.Value, description));
    }

    public int AddMinimumAgeRequirement(int minimumAge, string validationPolicy)
    {
        ThrowIfIdNotSet();
        
        var statementId = _nextPolicyStatementId;
        Raise(new RacePolicyMinimumAgeRequirementAdded(Id, statementId, minimumAge, validationPolicy));
        return statementId;
    }

    public int AddMaximumAgeRequirement(int maximumAge, string validationPolicy)
    {
        ThrowIfIdNotSet();
        
        var statementId = _nextPolicyStatementId;
        Raise(new RacePolicyMaximumAgeRequirementAdded(Id, statementId, maximumAge, validationPolicy));
        return statementId;
    }

    public int AddInsuranceProviderRequirement(string insuranceProvider, string validationPolicy)
    {
        ThrowIfIdNotSet();
        
        var statementId = _nextPolicyStatementId;
        Raise(new RacePolicyInsuranceProviderRequirementAdded(Id, statementId, insuranceProvider, validationPolicy));
        return statementId;
    }

    public int AddGovernmentDocumentValidationRequirement(string governmentDocument, string validationPolicy)
    {
        ThrowIfIdNotSet();
        
        var statementId = _nextPolicyStatementId;
        Raise(new RacePolicyGovernmentDocumentValidationRequirementAdded(Id, statementId, governmentDocument, validationPolicy));
        return statementId;
    }

    public int AddClubRequirement(Guid clubId)
    {
        ThrowIfIdNotSet();
        
        var statementId = _nextPolicyStatementId;
        Raise(new RacePolicyClubRequirementAdded(Id, statementId, clubId));
        return statementId;
    }

    public int AddClubMembershipLevelRequirement(Guid clubId, int membershipLevel)
    {
        ThrowIfIdNotSet();
        
        var statementId = _nextPolicyStatementId;
        Raise(new RacePolicyClubMembershipLevelRequirementAdded(Id, statementId, clubId, membershipLevel));
        return statementId;
    }

    public int AddPolicyStatement(int leftHandStatementId, string operand, int rightHandStatementId, bool isWithinBrackets)
    {
        ThrowIfIdNotSet();
        
        if (!_statementTypes.ContainsKey(leftHandStatementId))
            throw new InvalidOperationException($"Left hand statement {leftHandStatementId} does not exist.");
        if (!_statementTypes.ContainsKey(rightHandStatementId))
            throw new InvalidOperationException($"Right hand statement {rightHandStatementId} does not exist.");

        ValidateStatementBranchForNoRecursion(leftHandStatementId, rightHandStatementId);
        ValidateStatementBranchForNoRecursion(rightHandStatementId, leftHandStatementId);

        var statementId = _nextPolicyStatementId;
        Raise(new RacePolicyStatementAdded(Id, statementId, leftHandStatementId, operand, rightHandStatementId, isWithinBrackets));
        return statementId;
    }

    public void RemovePolicyStatement(int policyStatementId)
    {
        ThrowIfIdNotSet();
        if (!_compositeStatements.ContainsKey(policyStatementId))
            throw new InvalidOperationException($"Policy statement {policyStatementId} does not exist.");
        
        ValidateStatementNotReferenced(policyStatementId);

        Raise(new RacePolicyStatementRemoved(Id, policyStatementId));
    }

    public void SetRootStatement(int rootPolicyStatementId)
    {
        ThrowIfIdNotSet();
        if (!_statementTypes.ContainsKey(rootPolicyStatementId))
            throw new InvalidOperationException($"Statement {rootPolicyStatementId} does not exist.");

        Raise(new RacePolicyRootStatementSet(Id, rootPolicyStatementId));
    }
    private void ValidateStatementBranchForNoRecursion(int validateStatementId, int branchStatementId)
    {
        if(validateStatementId == branchStatementId)
            throw new InvalidOperationException($"Cannot reference statement {branchStatementId} in a way that would create a recursive loop.");

        if (!_statementTypes.TryGetValue(branchStatementId, out var statementType) || statementType != StatementType.Composite)
        {
            return;
        }

        var branchStatement = _compositeStatements[branchStatementId];

        ValidateStatementBranchForNoRecursion(validateStatementId, branchStatement.LeftHandStatementId);
        ValidateStatementBranchForNoRecursion(validateStatementId, branchStatement.RightHandStatementId);
    }
    private void ValidateStatementNotReferenced(int statementId)
    {
        // Check if this statement is referenced by any composite statements
        foreach (var composite in _compositeStatements.Values)
        {
            if (composite.LeftHandStatementId == statementId || composite.RightHandStatementId == statementId)
                throw new InvalidOperationException($"Cannot remove statement {statementId} because it is referenced by composite statements.");
        }

        // Check if this statement is the root statement
        if (_rootPolicyStatementId == statementId)
            throw new InvalidOperationException($"Cannot remove statement {statementId} because it is the root statement.");
    }

    // Event handlers
    private void When(RacePolicyCreated e)
    {
        Id = e.RacePolicyId;
        _clubId = e.ClubId;
        _name = ValueTypes.Name.Rehydrate(e.Name);
        _description = e.Description;
    }

    private void When(RacePolicyMinimumAgeRequirementAdded e)
    {
        _minimumAgeRequirements[e.PolicyStatementId] = new MinimumAgeRequirement
        {
            MinimumAge = e.MinimumAge,
            ValidationPolicy = e.ValidationPolicy
        };
        _statementTypes[e.PolicyStatementId] = StatementType.MinimumAge;
        
        if (e.PolicyStatementId >= _nextPolicyStatementId)
            _nextPolicyStatementId = e.PolicyStatementId + 1;
    }

    private void When(RacePolicyMaximumAgeRequirementAdded e)
    {
        _maximumAgeRequirements[e.PolicyStatementId] = new MaximumAgeRequirement
        {
            MaximumAge = e.MaximumAge,
            ValidationPolicy = e.ValidationPolicy
        };
        _statementTypes[e.PolicyStatementId] = StatementType.MaximumAge;
        
        if (e.PolicyStatementId >= _nextPolicyStatementId)
            _nextPolicyStatementId = e.PolicyStatementId + 1;
    }

    private void When(RacePolicyInsuranceProviderRequirementAdded e)
    {
        _insuranceProviderRequirements[e.PolicyStatementId] = new InsuranceProviderRequirement
        {
            InsuranceProvider = e.InsuranceProvider,
            ValidationPolicy = e.ValidationPolicy
        };
        _statementTypes[e.PolicyStatementId] = StatementType.InsuranceProvider;
        
        if (e.PolicyStatementId >= _nextPolicyStatementId)
            _nextPolicyStatementId = e.PolicyStatementId + 1;
    }

    private void When(RacePolicyGovernmentDocumentValidationRequirementAdded e)
    {
        _governmentDocumentRequirements[e.PolicyStatementId] = new GovernmentDocumentRequirement
        {
            GovernmentDocument = e.GovernmentDocument,
            ValidationPolicy = e.ValidationPolicy
        };
        _statementTypes[e.PolicyStatementId] = StatementType.GovernmentDocument;
        
        if (e.PolicyStatementId >= _nextPolicyStatementId)
            _nextPolicyStatementId = e.PolicyStatementId + 1;
    }

    private void When(RacePolicyClubRequirementAdded e)
    {
        _clubRequirements[e.PolicyStatementId] = new ClubRequirement
        {
            ClubId = e.ClubId
        };
        _statementTypes[e.PolicyStatementId] = StatementType.ClubMembership;
        
        if (e.PolicyStatementId >= _nextPolicyStatementId)
            _nextPolicyStatementId = e.PolicyStatementId + 1;
    }

    private void When(RacePolicyClubMembershipLevelRequirementAdded e)
    {
        _clubMembershipLevelRequirements[e.PolicyStatementId] = new ClubMembershipLevelRequirement
        {
            ClubId = e.ClubId,
            MembershipLevel = e.MembershipLevel
        };
        _statementTypes[e.PolicyStatementId] = StatementType.ClubMembershipLevel;
        
        if (e.PolicyStatementId >= _nextPolicyStatementId)
            _nextPolicyStatementId = e.PolicyStatementId + 1;
    }
    private void When(RacePolicyStatementAdded e)
    {
        _compositeStatements[e.PolicyStatementId] = new Statement
        {
            LeftHandStatementId = e.LeftHandStatementId,
            Operand = e.Operand,
            RightHandStatementId = e.RightHandStatementId,
            IsWithinBrackets = e.IsWithinBrackets
        };
        _statementTypes[e.PolicyStatementId] = StatementType.Composite;
        
        if (e.PolicyStatementId >= _nextPolicyStatementId)
            _nextPolicyStatementId = e.PolicyStatementId + 1;
    }

    private void When(RacePolicyStatementRemoved e)
    {
        _compositeStatements.Remove(e.PolicyStatementId);
        _statementTypes.Remove(e.PolicyStatementId);
    }

    private void When(RacePolicyRootStatementSet e)
    {
        _rootPolicyStatementId = e.RootPolicyStatementId;
    }
}
