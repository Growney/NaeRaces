using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetRacePackageRegistrationStatusRequest
{
    [Required]
    public bool IsOpen { get; set; }
}
