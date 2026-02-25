using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Pilot;

public class RegisterPilotRequest
{
    [Required]
    public Guid PilotId { get; set; }

    [Required]
    public string CallSign { get; set; } = string.Empty;
}
