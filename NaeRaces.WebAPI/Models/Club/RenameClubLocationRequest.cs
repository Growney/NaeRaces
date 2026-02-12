using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class RenameClubLocationRequest
{
    [Required]
    public string NewLocationName { get; set; } = string.Empty;
}
