using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class TagRaceRequest
{
    [Required]
    public string Tag { get; set; } = string.Empty;
}
