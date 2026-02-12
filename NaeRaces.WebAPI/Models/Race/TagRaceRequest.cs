using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class TagRaceRequest
{
    [Required]
    public string Tag { get; set; } = string.Empty;
}
