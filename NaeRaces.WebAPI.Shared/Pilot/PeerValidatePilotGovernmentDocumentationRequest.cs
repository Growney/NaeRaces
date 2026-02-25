using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Pilot;

public class PeerValidatePilotGovernmentDocumentationRequest
{
    [Required]
    [MaxLength(50)]
    public string GovernmentDocument { get; set; } = string.Empty;

    [Required]
    public DateTime ValidUntil { get; set; }

    [Required]
    public Guid ValidatedByPilotId { get; set; }
}
