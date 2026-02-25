using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

public class SetRootStatementRequest
{
    [Required]
    public int RootPolicyStatementId { get; set; }
}
