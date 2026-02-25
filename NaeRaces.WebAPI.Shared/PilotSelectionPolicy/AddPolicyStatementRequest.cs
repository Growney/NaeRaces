using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

public class AddPolicyStatementRequest
{
    [Required]
    public int LeftHandStatementId { get; set; }

    [Required]
    public string Operand { get; set; } = string.Empty;

    [Required]
    public int RightHandStatementId { get; set; }

    [Required]
    public bool IsWithinBrackets { get; set; }
}
