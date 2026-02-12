using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class UpdateClubDescriptionRequest
{
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}
