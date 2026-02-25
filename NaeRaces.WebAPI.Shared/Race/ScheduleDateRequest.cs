using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class ScheduleDateRequest
{
    [Required]
    public DateTime Date { get; set; }
}
