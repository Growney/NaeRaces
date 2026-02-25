using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Team;

public class AddPilotToTeamRequest
{
    [Required]
    public Guid PilotId { get; set; }
}
