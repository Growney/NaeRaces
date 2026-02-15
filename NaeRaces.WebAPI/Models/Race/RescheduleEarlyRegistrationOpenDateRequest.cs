using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class RescheduleEarlyRegistrationOpenDateRequest
{
    [Required]
    public DateTime RegistrationOpenDate { get; set; }
}
