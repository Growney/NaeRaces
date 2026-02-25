using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetRaceCostRequest
{
    [Required]
    public string Currency { get; set; } = string.Empty;

    [Required]
    public decimal Cost { get; set; }
}
