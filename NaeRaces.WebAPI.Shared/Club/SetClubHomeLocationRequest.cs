using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class SetClubHomeLocationRequest
{
    [Required]
    public int LocationId { get; set; }
}
