using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.RaceSeries;

public class PlanRaceSeriesRequest
{
    [Required]
    public Guid RaceSeriesId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
