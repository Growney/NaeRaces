using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class RegisterIndividualPilotForRaceRequest
{
    [Required]
    public int RacePackageId { get; set; }
}

