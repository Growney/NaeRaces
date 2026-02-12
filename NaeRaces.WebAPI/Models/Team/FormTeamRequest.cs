using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Team;

public class FormTeamRequest
{
    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid CaptainPilotId { get; set; }
}
