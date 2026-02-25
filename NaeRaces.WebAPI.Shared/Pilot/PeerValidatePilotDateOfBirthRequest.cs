using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Pilot;

public class PeerValidatePilotDateOfBirthRequest
{
    [Required]
    public Guid ValidatedByPilotId { get; set; }
}
