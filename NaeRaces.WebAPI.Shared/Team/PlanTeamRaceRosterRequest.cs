using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Team;

public class PlanTeamRaceRosterRequest
{
    [Required]
    public int RosterId { get; set; }

    [Required]
    public Guid RaceId { get; set; }
}
