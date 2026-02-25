using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetAttendeesRequest
{
    [Required]
    public int Attendees { get; set; }
}
