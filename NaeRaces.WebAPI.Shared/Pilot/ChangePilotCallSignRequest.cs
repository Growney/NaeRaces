using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Pilot;

public class ChangePilotCallSignRequest
{
    [Required]
    public string NewCallSign { get; set; } = string.Empty;
}
