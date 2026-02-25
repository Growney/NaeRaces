using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class ScheduleRaceDateRequest
{
    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }
}
