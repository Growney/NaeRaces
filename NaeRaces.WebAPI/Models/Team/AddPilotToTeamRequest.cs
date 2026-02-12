using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Team;

public class AddPilotToTeamRequest
{
    [Required]
    public Guid PilotId { get; set; }
}
