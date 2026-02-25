using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetRaceValidationPolicyRequest
{
    [Required]
    public Guid ValidationPolicyId { get; set; }
}
