using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record RacePolicyCreated(Guid RacePolicyId, Guid ClubId, string Name, string Description);
public record RacePolicyMinimumAgeRequirementAdded(Guid RacePolicyId, int PolicyStatementId, int MinimumAge, string ValidationPolicy);
public record RacePolicyMaximumAgeRequirementAdded(Guid RacePolicyId, int PolicyStatementId, int MaximumAge, string ValidationPolicy);
public record RacePolicyInsuranceProviderRequirementAdded(Guid RacePolicyId, int PolicyStatementId, string InsuranceProvider, string ValidationPolicy);
public record RacePolicyGovernmentDocumentValidationRequirementAdded(Guid RacePolicyId, int PolicyStatementId, string GovernmentDocument, string ValidationPolicy);
public record RacePolicyClubRequirementAdded(Guid RacePolicyId, int PolicyStatementId, Guid ClubId);
public record RacePolicyClubMembershipLevelRequirementAdded(Guid RacePolicyId,int PolicyStatementId, Guid ClubId, int MembershipLevel);
public record RacePolicyStatementAdded(Guid RacePolicyId, int PolicyStatementId, int LeftHandStatementId, string Operand, int RightHandStatementId, bool IsWithinBrackets);
public record RacePolicyStatementRemoved(Guid RacePolicyId, int PolicyStatementId);
public record RacePolicyRootStatementSet(Guid RacePolicyId, int RootPolicyStatementId);