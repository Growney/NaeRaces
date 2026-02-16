using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record PilotSelectionPolicyCreated(Guid PilotSelectionPolicyId, Guid ClubId, string Name, string Description);
public record PilotSelectionPolicyMinimumAgeRequirementAdded(Guid PilotSelectionPolicyId, int PolicyStatementId, int MinimumAge, string ValidationPolicy);
public record PilotSelectionPolicyMaximumAgeRequirementAdded(Guid PilotSelectionPolicyId, int PolicyStatementId, int MaximumAge, string ValidationPolicy);
public record PilotSelectionPolicyInsuranceProviderRequirementAdded(Guid PilotSelectionPolicyId, int PolicyStatementId, string InsuranceProvider, string ValidationPolicy);
public record PilotSelectionPolicyGovernmentDocumentValidationRequirementAdded(Guid PilotSelectionPolicyId, int PolicyStatementId, string GovernmentDocument, string ValidationPolicy);
public record PilotSelectionPolicyClubRequirementAdded(Guid PilotSelectionPolicyId, int PolicyStatementId, Guid ClubId);
public record PilotSelectionPolicyClubMembershipLevelRequirementAdded(Guid PilotSelectionPolicyId,int PolicyStatementId, Guid ClubId, int MembershipLevel);
public record PilotSelectionPolicyStatementAdded(Guid PilotSelectionPolicyId, int PolicyStatementId, int LeftHandStatementId, string Operand, int RightHandStatementId, bool IsWithinBrackets);
public record PilotSelectionPolicyStatementRemoved(Guid PilotSelectionPolicyId, int PolicyStatementId);
public record PilotSelectionPolicyRootStatementSet(Guid PilotSelectionPolicyId, int RootPolicyStatementId);