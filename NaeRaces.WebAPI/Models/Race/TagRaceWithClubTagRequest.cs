using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class TagRaceWithClubTagRequest
{
    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public string Tag { get; set; } = string.Empty;
}
