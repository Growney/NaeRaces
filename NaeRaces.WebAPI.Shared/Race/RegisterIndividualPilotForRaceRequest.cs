using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class RegisterIndividualPilotForRaceRequest
{
    [Required]
    public Guid PilotId { get; set; }

    [Required]
    public Guid RegistrationId { get; set; }

    [Required]
    public int RacePackageId { get; set; }
}

