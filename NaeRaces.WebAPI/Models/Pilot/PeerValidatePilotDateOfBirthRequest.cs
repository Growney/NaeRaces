using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Pilot;

public class PeerValidatePilotDateOfBirthRequest
{
    [Required]
    public Guid ValidatedByPilotId { get; set; }
}
