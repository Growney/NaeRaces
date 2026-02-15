using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class RegisterTeamRosterForRaceRequest
{
    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public IEnumerable<Guid> PilotIds { get; set; } = Enumerable.Empty<Guid>();

    [Required]
    public Guid RegistrationId { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;
}

