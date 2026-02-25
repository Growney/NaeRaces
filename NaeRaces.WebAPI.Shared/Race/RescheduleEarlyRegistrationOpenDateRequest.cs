using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class RescheduleEarlyRegistrationOpenDateRequest
{
    [Required]
    public DateTime RegistrationOpenDate { get; set; }
}
