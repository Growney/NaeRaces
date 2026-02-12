using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class AddClubRaceTagRequest
{
    [Required]
    public string Tag { get; set; } = string.Empty;

    [Required]
    public string Colour { get; set; } = string.Empty;
}
