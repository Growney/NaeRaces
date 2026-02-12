using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetTeamsRequest
{
    [Required]
    public int Teams { get; set; }
}
