using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Team;

public class AddPilotToTeamRosterRequest
{
    [Required]
    public Guid PilotId { get; set; }
}
