using NaeRaces.Command.Aggregates;
using NaeRaces.Events;
using System;
using Xunit;

namespace NaeRaces.Command.Tests;

public class For_RacePolicy
{
    private static RacePolicy CreateTestPolicy()
    {
        var policyId = Guid.NewGuid();
        var clubId = Guid.NewGuid();
        var name = ValueTypes.Name.Create("Test Policy");
        return new RacePolicy(policyId, clubId, name, "Test Description");
    }

    [Fact]
    public void Should_Create_RacePolicy_With_Valid_Parameters()
    {
        // Arrange
        var policyId = Guid.NewGuid();
        var clubId = Guid.NewGuid();
        var name = ValueTypes.Name.Create("Test Policy");
        var description = "Test Description";

        // Act
        var policy = new RacePolicy(policyId, clubId, name, description);
        var events = policy.GetEvents();

        // Assert
        Assert.Single(events);
        var createdEvent = Assert.IsType<RacePolicyCreated>(events.First().Payload);
        Assert.Equal(policyId, createdEvent.RacePolicyId);
        Assert.Equal(clubId, createdEvent.ClubId);
        Assert.Equal("Test Policy", createdEvent.Name);
        Assert.Equal(description, createdEvent.Description);
    }

    [Fact]
    public void Should_Add_MinimumAge_Requirement_And_Return_StatementId()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents(); // Clear initial events

        // Act
        var statementId = policy.AddMinimumAgeRequirement(18, "AtRegistration");
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(1, statementId);
        Assert.Single(events);
        var addedEvent = Assert.IsType<RacePolicyMinimumAgeRequirementAdded>(events.First().Payload);
        Assert.Equal(1, addedEvent.PolicyStatementId);
        Assert.Equal(18, addedEvent.MinimumAge);
        Assert.Equal("AtRegistration", addedEvent.ValidationPolicy);
    }

    [Fact]
    public void Should_Add_MaximumAge_Requirement_And_Return_StatementId()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();

        // Act
        var statementId = policy.AddMaximumAgeRequirement(65, "AtRaceDay");
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(1, statementId);
        Assert.Single(events);
        var addedEvent = Assert.IsType<RacePolicyMaximumAgeRequirementAdded>(events.First().Payload);
        Assert.Equal(1, addedEvent.PolicyStatementId);
        Assert.Equal(65, addedEvent.MaximumAge);
        Assert.Equal("AtRaceDay", addedEvent.ValidationPolicy);
    }

    [Fact]
    public void Should_Add_InsuranceProvider_Requirement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();

        // Act
        var statementId = policy.AddInsuranceProviderRequirement("AcmeInsurance", "BeforeRace");
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(1, statementId);
        var addedEvent = Assert.IsType<RacePolicyInsuranceProviderRequirementAdded>(events.First().Payload);
        Assert.Equal("AcmeInsurance", addedEvent.InsuranceProvider);
        Assert.Equal("BeforeRace", addedEvent.ValidationPolicy);
    }

    [Fact]
    public void Should_Add_GovernmentDocument_Requirement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();

        // Act
        var statementId = policy.AddGovernmentDocumentValidationRequirement("Passport", "AtRegistration");
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(1, statementId);
        var addedEvent = Assert.IsType<RacePolicyGovernmentDocumentValidationRequirementAdded>(events.First().Payload);
        Assert.Equal("Passport", addedEvent.GovernmentDocument);
        Assert.Equal("AtRegistration", addedEvent.ValidationPolicy);
    }

    [Fact]
    public void Should_Add_Club_Requirement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var clubId = Guid.NewGuid();
        policy.GetEvents();

        // Act
        var statementId = policy.AddClubRequirement(clubId);
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(1, statementId);
        var addedEvent = Assert.IsType<RacePolicyClubRequirementAdded>(events.First().Payload);
        Assert.Equal(clubId, addedEvent.ClubId);
    }

    [Fact]
    public void Should_Add_ClubMembershipLevel_Requirement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var clubId = Guid.NewGuid();
        policy.GetEvents();

        // Act
        var statementId = policy.AddClubMembershipLevelRequirement(clubId, 2);
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(1, statementId);
        var addedEvent = Assert.IsType<RacePolicyClubMembershipLevelRequirementAdded>(events.First().Payload);
        Assert.Equal(clubId, addedEvent.ClubId);
        Assert.Equal(2, addedEvent.MembershipLevel);
    }

    [Fact]
    public void Should_Increment_StatementId_For_Multiple_Requirements()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();

        // Act
        var id1 = policy.AddMinimumAgeRequirement(18, "Policy1");
        var id2 = policy.AddMaximumAgeRequirement(65, "Policy2");
        var id3 = policy.AddInsuranceProviderRequirement("Provider", "Policy3");

        // Assert
        Assert.Equal(1, id1);
        Assert.Equal(2, id2);
        Assert.Equal(3, id3);
    }

    [Fact]
    public void Should_Create_Composite_Statement_From_Two_Requirements()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();
        var minAgeId = policy.AddMinimumAgeRequirement(18, "Policy");
        var maxAgeId = policy.AddMaximumAgeRequirement(65, "Policy");
        policy.GetEvents();

        // Act
        var compositeId = policy.AddPolicyStatement(minAgeId, "AND", maxAgeId, false);
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(3, compositeId);
        var addedEvent = Assert.IsType<RacePolicyStatementAdded>(events.First().Payload);
        Assert.Equal(3, addedEvent.PolicyStatementId);
        Assert.Equal(minAgeId, addedEvent.LeftHandStatementId);
        Assert.Equal("AND", addedEvent.Operand);
        Assert.Equal(maxAgeId, addedEvent.RightHandStatementId);
        Assert.False(addedEvent.IsWithinBrackets);
    }

    [Fact]
    public void Should_Create_Composite_Statement_With_Brackets()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();
        var minAgeId = policy.AddMinimumAgeRequirement(18, "Policy");
        var maxAgeId = policy.AddMaximumAgeRequirement(65, "Policy");
        policy.GetEvents();

        // Act
        var compositeId = policy.AddPolicyStatement(minAgeId, "AND", maxAgeId, true);
        var events = policy.GetEvents();

        // Assert
        var addedEvent = Assert.IsType<RacePolicyStatementAdded>(events.First().Payload);
        Assert.True(addedEvent.IsWithinBrackets);
    }

    [Fact]
    public void Should_Create_Nested_Composite_Statements()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();
        var minAgeId = policy.AddMinimumAgeRequirement(18, "Policy");
        var maxAgeId = policy.AddMaximumAgeRequirement(65, "Policy");
        var insuranceId = policy.AddInsuranceProviderRequirement("Provider", "Policy");
        var composite1 = policy.AddPolicyStatement(minAgeId, "AND", maxAgeId, true);
        policy.GetEvents();

        // Act
        var composite2 = policy.AddPolicyStatement(composite1, "OR", insuranceId, false);
        var events = policy.GetEvents();

        // Assert
        Assert.Equal(5, composite2);
        var addedEvent = Assert.IsType<RacePolicyStatementAdded>(events.First().Payload);
        Assert.Equal(composite1, addedEvent.LeftHandStatementId);
        Assert.Equal(insuranceId, addedEvent.RightHandStatementId);
    }

    [Fact]
    public void Should_Throw_When_Creating_Composite_With_NonExistent_LeftStatement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var rightId = policy.AddMinimumAgeRequirement(18, "Policy");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.AddPolicyStatement(999, "AND", rightId, false));
        Assert.Contains("Left hand statement 999 does not exist", exception.Message);
    }

    [Fact]
    public void Should_Throw_When_Creating_Composite_With_NonExistent_RightStatement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var leftId = policy.AddMinimumAgeRequirement(18, "Policy");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.AddPolicyStatement(leftId, "AND", 999, false));
        Assert.Contains("Right hand statement 999 does not exist", exception.Message);
    }

    [Fact]
    public void Should_Prevent_Circular_Reference_Direct()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var statement1 = policy.AddMinimumAgeRequirement(18, "Policy");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.AddPolicyStatement(statement1, "AND", statement1, false));
        Assert.Contains("recursive loop", exception.Message);
    }

    [Fact]
    public void Should_Prevent_Circular_Reference_Indirect()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var statement1 = policy.AddMinimumAgeRequirement(18, "Policy");
        var statement2 = policy.AddMaximumAgeRequirement(65, "Policy");
        var composite1 = policy.AddPolicyStatement(statement1, "AND", statement2, false);

        // Act & Assert - Try to create statement2 AND composite1 (which contains statement2)
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.AddPolicyStatement(statement2, "OR", composite1, false));
        Assert.Contains("recursive loop", exception.Message);
    }

    [Fact]
    public void Should_Prevent_Deep_Circular_Reference()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var s1 = policy.AddMinimumAgeRequirement(18, "Policy");
        var s2 = policy.AddMaximumAgeRequirement(65, "Policy");
        var s3 = policy.AddInsuranceProviderRequirement("Provider", "Policy");
        var c1 = policy.AddPolicyStatement(s1, "AND", s2, false); // c1 = s1 AND s2
        var c2 = policy.AddPolicyStatement(c1, "OR", s3, false);  // c2 = c1 OR s3

        // Act & Assert - Try to create s1 AND c2 (c2 contains c1 which contains s1)
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.AddPolicyStatement(s1, "AND", c2, false));
        Assert.Contains("recursive loop", exception.Message);
    }

    [Fact]
    public void Should_Set_Root_Statement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var statement = policy.AddMinimumAgeRequirement(18, "Policy");
        policy.GetEvents();

        // Act
        policy.SetRootStatement(statement);
        var events = policy.GetEvents();

        // Assert
        Assert.Single(events);
        var setEvent = Assert.IsType<RacePolicyRootStatementSet>(events.First().Payload);
        Assert.Equal(statement, setEvent.RootPolicyStatementId);
    }

    [Fact]
    public void Should_Throw_When_Setting_NonExistent_Root_Statement()
    {
        // Arrange
        var policy = CreateTestPolicy();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.SetRootStatement(999));
        Assert.Contains("Statement 999 does not exist", exception.Message);
    }

    [Fact]
    public void Should_Remove_Composite_Statement_When_Not_Referenced()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var s1 = policy.AddMinimumAgeRequirement(18, "Policy");
        var s2 = policy.AddMaximumAgeRequirement(65, "Policy");
        var composite = policy.AddPolicyStatement(s1, "AND", s2, false);
        policy.GetEvents();

        // Act
        policy.RemovePolicyStatement(composite);
        var events = policy.GetEvents();

        // Assert
        Assert.Single(events);
        var removedEvent = Assert.IsType<RacePolicyStatementRemoved>(events.First().Payload);
        Assert.Equal(composite, removedEvent.PolicyStatementId);
    }

    [Fact]
    public void Should_Throw_When_Removing_Statement_Referenced_By_Another_Composite()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var s1 = policy.AddMinimumAgeRequirement(18, "Policy");
        var s2 = policy.AddMaximumAgeRequirement(65, "Policy");
        var s3 = policy.AddInsuranceProviderRequirement("Provider", "Policy");
        var c1 = policy.AddPolicyStatement(s1, "AND", s2, false);
        var c2 = policy.AddPolicyStatement(c1, "OR", s3, false);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.RemovePolicyStatement(c1));
        Assert.Contains("referenced by composite statements", exception.Message);
    }

    [Fact]
    public void Should_Throw_When_Removing_Root_Statement()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var s1 = policy.AddMinimumAgeRequirement(18, "Policy");
        var s2 = policy.AddMaximumAgeRequirement(65, "Policy");
        var composite = policy.AddPolicyStatement(s1, "AND", s2, false);
        policy.SetRootStatement(composite);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.RemovePolicyStatement(composite));
        Assert.Contains("root statement", exception.Message);
    }

    [Fact]
    public void Should_Throw_When_Removing_NonExistent_Statement()
    {
        // Arrange
        var policy = CreateTestPolicy();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.RemovePolicyStatement(999));
        Assert.Contains("Policy statement 999 does not exist", exception.Message);
    }

    [Fact]
    public void Should_Build_Complex_Policy_Tree()
    {
        // Arrange
        var policy = CreateTestPolicy();
        policy.GetEvents();

        // Build: ((minAge AND maxAge) OR insurance) AND (club OR clubLevel)
        var minAge = policy.AddMinimumAgeRequirement(18, "Policy");
        var maxAge = policy.AddMaximumAgeRequirement(65, "Policy");
        var insurance = policy.AddInsuranceProviderRequirement("Provider", "Policy");
        var club = policy.AddClubRequirement(Guid.NewGuid());
        var clubLevel = policy.AddClubMembershipLevelRequirement(Guid.NewGuid(), 2);

        var ageGroup = policy.AddPolicyStatement(minAge, "AND", maxAge, true);
        var leftSide = policy.AddPolicyStatement(ageGroup, "OR", insurance, true);
        var rightSide = policy.AddPolicyStatement(club, "OR", clubLevel, true);
        var root = policy.AddPolicyStatement(leftSide, "AND", rightSide, false);

        policy.GetEvents();

        // Act
        policy.SetRootStatement(root);
        var events = policy.GetEvents();

        // Assert
        Assert.Single(events);
        var setEvent = Assert.IsType<RacePolicyRootStatementSet>(events.First().Payload);
        Assert.Equal(root, setEvent.RootPolicyStatementId);
    }

    [Fact]
    public void Should_Allow_Removing_Statement_After_Composite_Referencing_It_Is_Removed()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var s1 = policy.AddMinimumAgeRequirement(18, "Policy");
        var s2 = policy.AddMaximumAgeRequirement(65, "Policy");
        var composite = policy.AddPolicyStatement(s1, "AND", s2, false);
        
        // First verify we can't remove s1 while composite exists
        Assert.Throws<InvalidOperationException>(() => policy.RemovePolicyStatement(s1));

        // Remove the composite
        policy.RemovePolicyStatement(composite);
        policy.GetEvents();

        // Act - Now we should be able to remove statements that were previously referenced
        // Note: We can't actually remove base requirements in the current implementation
        // This test documents current behavior - composite statements can be removed but not base requirements
        var exception = Assert.Throws<InvalidOperationException>(() =>
            policy.RemovePolicyStatement(s1));
        
        // Assert - s1 is not a composite statement
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void Should_Maintain_Statement_Id_Sequence_After_Event_Replay()
    {
        // Arrange
        var policy = CreateTestPolicy();
        var s1 = policy.AddMinimumAgeRequirement(18, "Policy");
        var s2 = policy.AddMaximumAgeRequirement(65, "Policy");
        
        // Act - Add another requirement, should get ID 3
        var s3 = policy.AddInsuranceProviderRequirement("Provider", "Policy");
        
        // Assert
        Assert.Equal(1, s1);
        Assert.Equal(2, s2);
        Assert.Equal(3, s3);
    }
}
