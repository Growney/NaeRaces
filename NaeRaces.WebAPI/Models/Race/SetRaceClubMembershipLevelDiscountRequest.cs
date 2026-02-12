using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetRaceClubMembershipLevelDiscountRequest
{
    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public int MembershipLevelId { get; set; }

    [Required]
    public decimal Discount { get; set; }
}
