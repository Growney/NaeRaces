using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetEarlyRegistrationPolicyRequest
{
    [Required]
    public Guid PilotPolicyId { get; set; }
}
