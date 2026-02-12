using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetRaceValidationPolicyRequest
{
    [Required]
    public Guid ValidationPolicyId { get; set; }
}
