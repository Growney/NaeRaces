using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.RaceSeries;

public class AddRaceToSeriesRequest
{
    [Required]
    public Guid RaceId { get; set; }
}
