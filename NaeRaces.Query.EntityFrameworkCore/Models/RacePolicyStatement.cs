using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

[PrimaryKey(nameof(PolicyId), nameof(StatementId))]
public class RacePolicyStatement
{
    public Guid PolicyId { get; set; }
    public int StatementId { get; set; }
    public string StatementType { get; set; } = string.Empty;
    public int? LeftHandStatementId { get; set; }
    public string? Operand { get; set; }
    public int? RightHandStatementId { get; set; }
    public bool? IsWithinBrackets { get; set; }
    
    // Requirement properties
    public int? MinimumAge { get; set; }
    public int? MaximumAge { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? GovernmentDocument { get; set; }
    public Guid? RequiredClubId { get; set; }
    public int? RequiredMembershipLevelId { get; set; }
    public string? ValidationPolicy { get; set; }
}
