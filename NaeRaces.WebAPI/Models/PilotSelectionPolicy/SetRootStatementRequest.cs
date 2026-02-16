using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.PilotSelectionPolicy;

public class SetRootStatementRequest
{
    [Required]
    public int RootPolicyStatementId { get; set; }
}
