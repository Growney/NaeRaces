using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Team;

public class PlanTeamRaceRosterRequest
{
    [Required]
    public int RosterId { get; set; }

    [Required]
    public Guid RaceId { get; set; }
}
