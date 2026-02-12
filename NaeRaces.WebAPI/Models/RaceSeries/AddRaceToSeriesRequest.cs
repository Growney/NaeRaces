using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.RaceSeries;

public class AddRaceToSeriesRequest
{
    [Required]
    public Guid RaceId { get; set; }
}
