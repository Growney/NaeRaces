namespace NaeRaces.Query.Models;

public record PilotPolicyStatementDetails(
    Guid PolicyId,
    int StatementId,
    string StatementType,
    int? LeftHandStatementId,
    string? Operand,
    int? RightHandStatementId,
    bool? IsWithinBrackets,
    int? MinimumAge,
    int? MaximumAge,
    string? InsuranceProvider,
    string? GovernmentDocument,
    Guid? RequiredClubId,
    int? RequiredMembershipLevelId,
    string? ValidationPolicy);
