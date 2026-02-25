using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class RenameClubLocationRequest
{
    [Required]
    public string NewLocationName { get; set; } = string.Empty;
}
