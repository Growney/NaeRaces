using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Pilot;

public class ChangePilotCallSignRequest
{
    [Required]
    public string NewCallSign { get; set; } = string.Empty;
}
