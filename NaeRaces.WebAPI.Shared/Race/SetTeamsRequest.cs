using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetTeamsRequest
{
    [Required]
    public int Teams { get; set; }
}
