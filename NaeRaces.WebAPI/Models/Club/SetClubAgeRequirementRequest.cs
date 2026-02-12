using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class SetClubAgeRequirementRequest
{
    [Required]
    public int Age { get; set; }

    [Required]
    public string ValidationPolicy { get; set; } = string.Empty;
}
