using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class SetClubHomeLocationRequest
{
    [Required]
    public int LocationId { get; set; }
}
