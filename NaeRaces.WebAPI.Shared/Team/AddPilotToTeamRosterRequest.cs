using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Team;

public class AddPilotToTeamRosterRequest
{
    [Required]
    public Guid PilotId { get; set; }
}
