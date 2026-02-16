using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class ScheduleEarlyRegistrationOpenDateRequest
{
    [Required]
    public DateTime RegistrationOpenDate { get; set; }

    [Required]
    public Guid? PilotPolicyId { get; set; }
}

