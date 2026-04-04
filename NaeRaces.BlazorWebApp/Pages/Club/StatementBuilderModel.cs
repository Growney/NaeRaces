using NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

namespace NaeRaces.BlazorWebApp.Pages.Club;

public class StatementBuilderModel
{
    public string Type { get; set; } = string.Empty;

    public int MinimumAge { get; set; } = 18;
    public int MaximumAge { get; set; } = 100;
    public string InsuranceProvider { get; set; } = string.Empty;
    public string GovernmentDocument { get; set; } = string.Empty;
    public Guid RequiredClubId { get; set; }
    public int RequiredMembershipLevelId { get; set; }
    public string ValidationPolicy { get; set; } = "ANY";

    public string Operand { get; set; } = "AND";
    public bool IsWithinBrackets { get; set; }
    public StatementBuilderModel? Left { get; set; }
    public StatementBuilderModel? Right { get; set; }

    public int ExistingStatementId { get; set; }

    public static readonly Dictionary<string, string> ValidationPolicyOptions = new()
    {
        ["NONE"] = "No validation",
        ["ANY"] = "Any",
        ["ANY_CLUB_MEMBER"] = "Any Club Member",
        ["ANY_CLUB_COMMITTEE_MEMBER"] = "Any Club Committee Member",
        ["POLICY_CLUB_MEMBER"] = "Policy Club Member",
        ["POLICY_CLUB_COMMITTEE_MEMBER"] = "Policy Club Committee Member",
        ["PILOT_CLUB_MEMBER"] = "Pilot Club Member",
        ["PILOT_CLUB_COMMITTEE_MEMBER"] = "Pilot Club Committee Member"
    };

    public static string GetStatementSummary(PilotPolicyStatementResponse stmt) => stmt.StatementType switch
    {
        "MinimumAge" => $"Min age \u2265 {stmt.MinimumAge} ({stmt.ValidationPolicy})",
        "MaximumAge" => $"Max age \u2264 {stmt.MaximumAge} ({stmt.ValidationPolicy})",
        "InsuranceProvider" => $"Insurance: {stmt.InsuranceProvider} ({stmt.ValidationPolicy})",
        "GovernmentDocument" => $"Gov doc: {stmt.GovernmentDocument} ({stmt.ValidationPolicy})",
        "ClubMembership" => $"Club member: {stmt.RequiredClubId}",
        "ClubMembershipLevel" => $"Club {stmt.RequiredClubId} / level {stmt.RequiredMembershipLevelId}",
        "Composite" => $"{(stmt.IsWithinBrackets == true ? "(" : "")}#{stmt.LeftHandStatementId} {stmt.Operand} #{stmt.RightHandStatementId}{(stmt.IsWithinBrackets == true ? ")" : "")}",
        _ => stmt.StatementType
    };
}

public class StatementCreatedResponse
{
    public int StatementId { get; set; }
}
