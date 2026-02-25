using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetRaceDescriptionRequest
{
    [Required]
    [MaxLength(3000)]
    public string Description { get; set; } = string.Empty;
}
