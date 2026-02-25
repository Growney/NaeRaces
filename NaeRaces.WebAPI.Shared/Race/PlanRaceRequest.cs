using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class PlanRaceRequest
{
    [Required]
    public Guid RaceId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public int LocationId { get; set; }
}
