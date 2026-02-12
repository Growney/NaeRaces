using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetTeamSizeRequest
{
    [Required]
    public int TeamSize { get; set; }
}
