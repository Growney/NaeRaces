using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class PlanTeamRaceRequest
{
    [Required]
    public Guid RaceId { get; set; }
    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int TeamSize { get; set; }
}
