using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Pilot;

public class PeerValidatePilotInsuranceRequest
{
    [Required]
    [MaxLength(50)]
    public string InsuranceProvider { get; set; } = string.Empty;

    [Required]
    public DateTime ValidUntil { get; set; }

    [Required]
    public Guid ValidatedByPilotId { get; set; }
}
