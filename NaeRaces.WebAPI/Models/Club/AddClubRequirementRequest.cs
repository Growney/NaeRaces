using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Club;

public class AddClubRequirementRequest
{
    [Required]
    public Guid ClubId { get; set; }
}
