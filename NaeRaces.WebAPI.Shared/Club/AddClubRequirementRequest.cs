using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class AddClubRequirementRequest
{
    [Required]
    public Guid ClubId { get; set; }
}
