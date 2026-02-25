using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class AddRacePackageRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Cost { get; set; }
}
