using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Team;

public class SubstituteRosterPilotRequest
{
    [Required]
    public Guid RaceId { get; set; }

    [Required]
    public Guid OriginalPilotId { get; set; }

    [Required]
    public Guid SubstitutePilotId { get; set; }
}
