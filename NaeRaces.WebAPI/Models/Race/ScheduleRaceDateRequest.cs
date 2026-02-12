using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class ScheduleRaceDateRequest
{
    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }
}
