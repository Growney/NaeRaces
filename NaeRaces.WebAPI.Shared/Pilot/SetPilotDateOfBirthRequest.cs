using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Pilot;

public class SetPilotDateOfBirthRequest
{
    [Required]
    public DateTime DateOfBirth { get; set; }
}
