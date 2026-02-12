using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.RacePolicy;

public class SetRootStatementRequest
{
    [Required]
    public int RootPolicyStatementId { get; set; }
}
