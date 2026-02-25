using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class MigrateRaceValidationPolicyRequest
{
    [Required]
    public Guid ValidationPolicyId { get; set; }
    [Required]
    public long Version { get; set; }
}
