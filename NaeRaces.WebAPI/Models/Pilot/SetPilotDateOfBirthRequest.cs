using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Pilot;

public class SetPilotDateOfBirthRequest
{
    [Required]
    public DateTime DateOfBirth { get; set; }
}
