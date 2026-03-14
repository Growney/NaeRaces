using Microsoft.EntityFrameworkCore;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

[PrimaryKey(nameof(PilotId), nameof(ClubId))]
public class PilotClubDetails
{
    public Guid PilotId { get; set; }
    public Guid ClubId { get; set; }
    public string? ClubName { get; set; }
    public string? ClubCode { get; set; }
    public string? HomeLocationName { get; set; }
    public string? HomeLocationAddressLine1 { get; set; }
    public string? HomeLocationAddressLine2 { get; set; }
    public string? HomeLocationCity { get; set; }
    public string? HomeLocationPostcode { get; set; }
    public string? HomeLocationCounty { get; set; }
    public int? MembershipLevelId { get; set; }
    public string? MembershipLevelName { get; set; }
    public DateTime? MembershipValidUntil { get; set; }
}
