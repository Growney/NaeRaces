using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetRacePackageRegistrationStatusRequest
{
    [Required]
    public bool IsOpen { get; set; }
}
