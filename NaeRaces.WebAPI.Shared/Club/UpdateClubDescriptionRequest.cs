using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class UpdateClubDescriptionRequest
{
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
}
