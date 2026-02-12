using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetAttendeesRequest
{
    [Required]
    public int Attendees { get; set; }
}
