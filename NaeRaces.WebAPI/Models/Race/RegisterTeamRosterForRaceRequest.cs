using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class RegisterTeamRosterForRaceRequest
{
    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public int RosterId { get; set; }

    [Required]
    public Guid RegistrationId { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;

    [Required]
    public double BasePrice { get; set; }

    [Required]
    public double Discount { get; set; }
}
