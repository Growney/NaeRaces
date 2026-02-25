using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class ScheduleEarlyRegistrationOpenDateRequest
{
    [Required]
    public DateTime RegistrationOpenDate { get; set; }

    [Required]
    public Guid? PilotPolicyId { get; set; }
}

