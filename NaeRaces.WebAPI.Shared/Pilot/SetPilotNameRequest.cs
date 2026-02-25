using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Pilot;

public class SetPilotNameRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
}
