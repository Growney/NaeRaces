using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class ScheduleDateRequest
{
    [Required]
    public DateTime Date { get; set; }
}
