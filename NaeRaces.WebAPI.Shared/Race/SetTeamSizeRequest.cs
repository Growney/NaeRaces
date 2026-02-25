using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetTeamSizeRequest
{
    [Required]
    public int TeamSize { get; set; }
}
