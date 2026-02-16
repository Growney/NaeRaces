using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetEarlyRegistrationPolicyRequest
{
    [Required]
    public Guid PilotPolicyId { get; set; }
}
